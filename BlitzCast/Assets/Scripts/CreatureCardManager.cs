using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Handles all of the displays and events that a card object does.
/// Inherits from CardManager and deals with display/cast/action specific to
/// CreatureCards.
/// The card itself is determined by Card.
/// </summary>
/// <seealso cref="Card"/>
/// <seealso cref="CardManager"/>
/// <seealso cref="SpellCardManager"/>
public class CreatureCardManager : CardManager
{

    // FOR REFERENCE:
    // Due to inheritance, the fields to the other properties already exist.
    // localPosition - the position in relation to the parent transform


    // the Entity is created/set after cast
    public Entity entity;

    // These fields are references to displayable components unique to Creature.
    // These are set through the Unity Editor and should already be set in the
    // prefab of the card.
    [SerializeField] private TMP_Text gridHealthText;
    [SerializeField] private TMP_Text gridActionValueText;
    [SerializeField] private TMP_Text gridSpeedText;
    [SerializeField] private Highlightable gridDisplayHighlightable;
    [SerializeField] private RectTransform gridDisplayRect;
    [SerializeField] private RectTransform gridStatusesParent;
    [SerializeField] private CircleTimer actionTimer;

    // size and offset for displaying
    private Vector2 spriteSize;
    // current action value/time of the Creature
    private int actionValue;
    private int actionTime;
    // the type-casted version of the card, because it is useful
    private CreatureCard creatureCard;
    // top-left coordinate in which creature casted onto
    private Vector2Int coordinates;


    /// <summary>
    /// Initialize this CardManager; set values and initiate displays.
    /// Also initiate displays specific to Creatures.
    /// </summary>
    /// <param name="card">
    /// The card which defines the values and behavior of this card object.
    /// </param>
    /// <param name="slot">
    /// The card hand slot which this card belongs to. DrawCard() will be
    /// called to this hand slot.
    /// </param>
    /// <param name="player">
    /// The reference to the player, which is needed to determine team.
    /// </param>
    public override void Initialize(Card card, HandSlot slot, PlayerManager player)
    {
        base.Initialize(card, slot, player);

        // set our private creatureCard variable to the type-casted version
        // of card for easy access
        // this is possible because of inheritance (CreatureCard is Card)
        creatureCard = (CreatureCard) card;

        // set the display texts/colors to their proper values
        actionValue = creatureCard.actionValue;
        actionTime = creatureCard.actionTime;

        // cellSize is based on the size of the grid cells
        // (and creature card size; ex. 2x1 -> 84x42)
        Vector2 cellSize = new Vector2(
            grid.cellsGroup.cellSize.x * creatureCard.size.y,
            grid.cellsGroup.cellSize.y * creatureCard.size.x
        );
        // spriteSize is based on the original size of our sprite
        // (and creature card size; ex. 2x1 -> 80x40)
        spriteSize = new Vector2(
            sprite.rectTransform.rect.width * creatureCard.size.y,
            sprite.rectTransform.rect.height * creatureCard.size.x
        );
        // sizeOffset is needed to calculate position shifted so that the pivot
        // is the top left corner of the sprite
        spriteOffset = new Vector2(
             grid.cellsGroup.cellSize.x / 2 * (creatureCard.size.y - 1),
            -grid.cellsGroup.cellSize.y / 2 * (creatureCard.size.x - 1)
        );

        // now set the actual size to the calculated size
        castingSpriteParent.sizeDelta = grid.cellsGroup.cellSize;
        sprite.rectTransform.sizeDelta = spriteSize;
        // add the calculated offset to local position
        sprite.transform.localPosition += (Vector3) spriteOffset;

        gridStatusesParent.sizeDelta = new Vector2(cellSize.x, gridStatusesParent.rect.y);
        gridDisplayRect.sizeDelta = cellSize;
        gridDisplayRect.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called every frame inside OnDrag(); for showing cast location through
    /// cell highlighting.  For Creature, also shows creature "shadow" sprite.
    /// </summary>
    public override void TryPreview()
    {
        GameObject target = GetCastLocation();
        // if the current mouse position points to a valid cast target position...
        if (target != null)
        {
            // set color/transparency ("shadow" sprite; black with transparency)
            sprite.color = new Color(0, 0, 0, 0.5f);
            // snap to target
            spriteMover.SetPosition(target.transform.position);
            // add cell highlighting
            foreach (GameObject targetObject in GetCastTargets(target))
            {
                Highlightable highlightable = targetObject.gameObject.GetComponent<Highlightable>();
                previewHighlightables.Add(highlightable);
                highlightable.Highlight(card.color);
            }
        }
        else
        {
            // reset the sprite color to normal (not "shadow")
            sprite.color = card.color;
        }
    }

    /// <summary>
    /// Get the GameObject of a valid cast location based on where the
    /// mouse cursor currently is.
    /// For Creatures, valid position is a GridCell where the creature fits
    /// (based on its size and CreatureGrid).
    /// </summary>
    /// <returns>
    /// The GameObject of a valid cast location; otherwise, null.
    /// </returns>
    public override GameObject GetCastLocation()
    {
        // Get first GridCell under cursor
        GameObject cellObject = gameManager.GetFirstUnderCursor<GridCell>();
        GridCell cell = cellObject != null ?
            cellObject.GetComponent<GridCell>() : null;

        // ensure cell is valid (cell exists, player side, creature fits)
        if (cell != null && (team == GameManager.Team.Friendly ?
                cell.coordinates.x >= grid.size.x / 2 :
                cell.coordinates.x < grid.size.x / 2) &&
            cell.coordinates.y + creatureCard.size.y - 1 < grid.size.y &&
            cell.coordinates.x + creatureCard.size.x - 1 < grid.size.x)
        {
            // if a creature already exists on one of the cells, exit
            for (int r = 0; r < creatureCard.size.x; r++)
            for (int c = 0; c < creatureCard.size.y; c++)
            {
                Vector2Int rc = new Vector2Int(cell.coordinates.x + r, cell.coordinates.y + c);
                if (grid.creatures.ContainsKey(rc))
                {
                    return null;
                }
            }

            // if reach here, target location is valid cell
            return cellObject;
        }
        return null;
    }

    /// <summary>
    /// Get the targets for the cast based on the location of the cast.
    /// </summary>
    /// <returns>
    /// List of GameObjects containing the target GameObjects.
    /// </returns>
    public override List<GameObject> GetCastTargets(GameObject locationObject)
    {
        List<GameObject> targets = new List<GameObject>();
        GridCell cell = locationObject.GetComponent<GridCell>();
        for (int r = 0; r < creatureCard.size.x; r++)
        for (int c = 0; c < creatureCard.size.y; c++)
        {
            Vector2Int rc = new Vector2Int(
                cell.coordinates.x + r,
                cell.coordinates.y + c
            );
            targets.Add(grid.GetCell(rc).gameObject);
        }
        return targets;
    }

    /// <summary>
    /// Start timer, add the creature location the grid, then, once the timer
    /// is complete, cast the creature onto the passed target cell.
    /// <para>
    /// Use with StartCoroutine(CastTimer).
    /// </para>
    /// </summary>
    /// <param name="target">
    /// The target location of the cast, as determined by GetCastLocation()
    /// previously.
    /// </param>
    protected override IEnumerator CastTimer(GameObject target)
    {
        // add creature location to grid
        foreach (GameObject cellObject in GetCastTargets(target))
        {
            GridCell c = cellObject.GetComponent<GridCell>();
            grid.creatures.Add(c.coordinates, this);
        }

        return base.CastTimer(target);
    }

    /// <summary>
    /// Cast this card onto a valid location (which should have been determined
    /// by GetCastLocation()).
    /// For Creature, create an Entity and activate the Creature on the grid.
    /// </summary>
    public override void Cast(GameObject locationObject)
    {
        GridCell cell = locationObject.GetComponent<GridCell>();
        coordinates = cell.coordinates;

        gameObject.layer = LayerMask.NameToLayer("Creatures");

        // Create entity
        entity = gameObject.AddComponent<Entity>();
        entity.Initialize(creatureCard.health, gridStatusesParent);
        entity.HealthChangeEvent += SetHealthDisplay;
        entity.SpeedChangeEvent += SetSpeedDisplay;
        if (creatureCard.statusModifiers != null && creatureCard.statusModifiers.Count > 0)
        {
            foreach (Entity.StatusModifier statusMod in creatureCard.statusModifiers)
            {
                entity.ApplyStatus(statusMod);
            }
        }
        if (creatureCard.statModifiers != null && creatureCard.statModifiers.Count > 0)
        {
            foreach (Entity.StatModifier statMod in creatureCard.statModifiers)
            {
                entity.ApplyStatModification(statMod);
            }
        }

        // Turn CreatureCard into Creature on grid
        gameObject.name = card.cardName;
        // set creature rect size (parent GameObject)
        RectTransform creatureRect = gameObject.GetComponent<RectTransform>();
        creatureRect.sizeDelta = spriteSize;
        // move out of the hierarchy
        castingSpriteParent.transform.SetParent(gridDisplayRect);
        castingSpriteParent.SetAsFirstSibling();
        castingSpriteParent.transform.localPosition = Vector3.zero;
        sprite.transform.localPosition = Vector3.zero;
        transform.SetParent(grid.playerCreaturesParent);
        transform.position = locationObject.transform.position;
        transform.localPosition += (Vector3) spriteOffset;

        // Disable Card display
        cardDisplay.gameObject.SetActive(false);
        // Enable Grid Creature display
        gridDisplayRect.gameObject.SetActive(true);
        gridHealthText.text = creatureCard.health.ToString();
        gridActionValueText.text = actionValue.ToString();
        gridStatusesParent.sizeDelta = new Vector2(spriteSize.x, 8);
        actionTimer.entity = entity;
        actionTimer.StartTimer(actionTime);
        animator.Initialize(
            card.name,
            "Cards/" + (card is CreatureCard ? "Creatures" : "Spells"),
            card.spriteAnimateSpeed,
            entity
        );
        sprite.gameObject.SetActive(true);
        sprite.color = card.color;

        // Start action timer coroutine
        StartCoroutine(ActionLoop());
    }


    /// <summary>
    /// Do some clean up, then destroy this gameObject.
    /// </summary>
    public override void DestroySelf()
    {
        // Delete creature locations from CreatureGrid
        for (int r = 0; r < creatureCard.size.x; r++)
        for (int c = 0; c < creatureCard.size.y; c++)
        {
            Vector2Int rc = new Vector2Int(coordinates.x + r, coordinates.y + c);
            grid.creatures.Remove(rc);
        }

        base.DestroySelf();
    }

    /// <summary>
    /// Get the targets for the action based on the location GameObject and the
    /// action type of this card.
    /// </summary>
    /// <returns>
    /// List of GameObjects containing the target GameObjects.
    /// </returns>
    public override HashSet<GameObject> GetActionTargets(GameObject locationObject)
    {
        // location is the upper left grid cell our creature is in

        //TODO: implement Creature Target Area

        //turn locations into targets
        HashSet<GameObject> targets = new HashSet<GameObject>();
        switch (creatureCard.targetArea)
        {
            default:
                Debug.LogWarning("Creature Target Area not implemented");
                break;
        }

        return targets;
    }

    /// <summary>
    /// Start action timer, then do action once timer is complete, reset timer,
    /// and loop while the creature Entity is alive.
    /// <para>
    /// Use with StartCoroutine(ActionLoop).
    /// </para>
    /// </summary>
    private IEnumerator ActionLoop()
    {
        while (entity.Health > 0)
        {
            if (actionTimer.IsComplete())
            {
                // Raise event to let entity know
                entity.TriggerActionEvent();

                // RNG chance of failure
                if (Random.Range(0, 100) > card.actionChance)
                {
                    Debug.Log(gameObject.name + " action failed");
                }
                else
                {
                    // get targets based on current cell
                    HashSet<GameObject> targets = GetActionTargets(grid.GetCell(coordinates).gameObject);
                    // filter targets by condition and execute action on each
                    targets = FilterTargetsByCondition(targets);
                    ExecuteActionOnTargets(targets);

                    // reset action timer countdown
                    actionTimer.StartTimer(creatureCard.actionTime);
                }

            }
            yield return null;
        }

        // when health <= 0
        DestroySelf();
    }

    /// <summary>
    /// Update the health display.
    /// This should be added onto HealthChangeEvent of the creature Entity.
    /// </summary>
    public void SetHealthDisplay(int oldHP, int newHP)
    {
        gridHealthText.text = newHP.ToString();
    }

    /// <summary>
    /// Update the speed display.
    /// This should be added onto SpeedChangeEvent of the creature Entity.
    /// </summary>
    public void SetSpeedDisplay(float oldSpeed, float newSpeed)
    {
        // if speed is ~1f
        if (Mathf.Abs(newSpeed - 1) < Mathf.Epsilon)
        {
            gridSpeedText.text = "";
        }
        else
        {
            gridSpeedText.text = "x" + (Mathf.Round(newSpeed * 10) / 10f).ToString();
        }
    }
}

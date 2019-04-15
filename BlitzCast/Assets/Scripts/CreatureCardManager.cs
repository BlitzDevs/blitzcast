using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CreatureCardManager : CardManager
{
    public Entity entity;

    [SerializeField] private TMP_Text cardHealthText;
    [SerializeField] private TMP_Text cardActionValueText;
    [SerializeField] private TMP_Text cardActionTimeText;
    [SerializeField] private RectTransform gridDisplayRect;
    [SerializeField] private Highlightable gridDisplayHighlightable;
    [SerializeField] private RectTransform gridStatusesParent;
    [SerializeField] private TMP_Text gridHealthText;
    [SerializeField] private TMP_Text gridActionValueText;
    [SerializeField] private TMP_Text gridSpeedText;
    [SerializeField] private CircleTimer actionTimer;

    private int actionValue;
    private int actionTime;
    private Vector2Int coordinates;
    private CreatureCard creatureCard;
    private Vector2 spriteSize;
    private Vector2 sizeOffset;


    // Initialize is our own function which is called by HandSlot
    public override void Initialize(Card card, HandSlot slot, PlayerManager player)
    {
        base.Initialize(card, slot, player);

        creatureCard = (CreatureCard) card;

        // set text on card
        actionValue = creatureCard.actionValue;
        actionTime = creatureCard.actionTime;
        cardHealthText.text = creatureCard.health.ToString();
        cardActionValueText.text = actionValue.ToString();
        cardActionTimeText.text = actionTime.ToString();

        Vector2 cellSize = new Vector2(
            grid.cellsGroup.cellSize.x * creatureCard.size.y,
            grid.cellsGroup.cellSize.y * creatureCard.size.x
        );
        spriteSize = new Vector2(
            sprite.rectTransform.rect.width * creatureCard.size.y,
            sprite.rectTransform.rect.height * creatureCard.size.x
        );
        sizeOffset = new Vector2(
             grid.cellsGroup.cellSize.x / 2 * (creatureCard.size.y - 1),
            -grid.cellsGroup.cellSize.y / 2 * (creatureCard.size.x - 1)
        );

        sprite.rectTransform.sizeDelta = spriteSize;
        sprite.transform.localPosition += (Vector3)sizeOffset;

        castingSpriteParent.sizeDelta = grid.cellsGroup.cellSize;

        gridStatusesParent.sizeDelta = new Vector2(cellSize.x, gridStatusesParent.rect.y);
        gridDisplayRect.sizeDelta = cellSize;
        gridDisplayRect.gameObject.SetActive(false);
    }

    public override void TryPreview()
    {
        GameObject target = GetCastLocation();
        if (target != null)
        {
            //set color/transparency
            sprite.color = new Color(0, 0, 0, 0.5f);
            //snap to target
            //castingSpriteParent.transform.position = target.transform.position;
            spriteMover.SetPosition(target.transform.position);

            foreach (GameObject targetObject in GetCastTargets(target))
            {
                Highlightable highlightable = targetObject.gameObject.GetComponent<Highlightable>();
                previewHighlightables.Add(highlightable);
                highlightable.Highlight(card.color);
            }
        }
        else
        {
            sprite.color = card.color;
        }
    }

    public override GameObject GetCastLocation()
    {
        // Get first GridCell under cursor
        GameObject cellObject = gameManager.GetFirstUnderCursor<GridCell>();
        GridCell cell = cellObject != null ?
            cellObject.GetComponent<GridCell>() : null;

        //will probably need to be changed as we implement player2 and networking
        if (cell != null && (team == GameManager.Team.Friendly ?
                cell.coordinates.x >= grid.size.x / 2 :
                cell.coordinates.x < grid.size.x / 2) &&
            cell.coordinates.y + creatureCard.size.y - 1 < grid.size.y &&
            cell.coordinates.x + creatureCard.size.x - 1 < grid.size.x)
        {
            //error checking to make sure all tiles are clear
            for (int r = 0; r < creatureCard.size.x; r++)
            for (int c = 0; c < creatureCard.size.y; c++)
            {
                Vector2Int rc = new Vector2Int(cell.coordinates.x + r, cell.coordinates.y + c);
                if (grid.creatures.ContainsKey(rc))
                {
                    return null;
                }
            }
            return cellObject;
        }
        return null;
    }

    public override List<GameObject> GetCastTargets(GameObject target)
    {
        List<GameObject> targets = new List<GameObject>();
        GridCell cell = target.GetComponent<GridCell>();
        for (int r = 0; r < creatureCard.size.x; r++)
        for (int c = 0; c < creatureCard.size.y; c++)
        {
            Vector2Int rc = new Vector2Int(
                cell.coordinates.x + r, cell.coordinates.y + c);
            targets.Add(grid.GetCell(rc).gameObject);
        }
        return targets;
    }

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

    public override void Cast(GameObject location)
    {
        GridCell cell = location.GetComponent<GridCell>();
        coordinates = cell.coordinates;

        gameObject.layer = LayerMask.NameToLayer("Creatures");

        // Create entity 
        entity = gameObject.AddComponent<Entity>();
        entity.Initialize(creatureCard.health, gridStatusesParent);
        entity.HealthChangeEvent += SetHealthDisplay;
        entity.SpeedChangeEvent += SetSpeedDisplay;
        if (creatureCard.statusModifiers != null && creatureCard.statusModifiers.Count > 0)
        {
            foreach (Entity.StatusModifier s in creatureCard.statusModifiers)
            {
                entity.ApplyStatus(s.statusType, s.stacks);
            }
        }
        if (creatureCard.statModifiers != null && creatureCard.statModifiers.Count > 0)
        {
            foreach (Entity.StatModifier s in creatureCard.statModifiers)
            {
                entity.ApplyStatModification(s.statChange, s.statChangeValue);
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
        transform.position = location.transform.position;
        transform.localPosition += (Vector3) sizeOffset;

        // Disable Card display
        SetTint(new Color(0f, 0f, 0f, 0f));
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        // Enable Grid Creature display
        gridDisplayRect.gameObject.SetActive(true);
        gridHealthText.text = creatureCard.health.ToString();
        gridActionValueText.text = actionValue.ToString();
        gridStatusesParent.sizeDelta = new Vector2(spriteSize.x, 8);
        actionTimer.entity = entity;
        actionTimer.StartTimer(actionTime);
        SpriteSheetAnimator.Animatable anim = new SpriteSheetAnimator.Animatable(
            card.name,
            "Cards/" + (card is CreatureCard ? "Creatures" : "Spells"),
            card.spriteAnimateSpeed,
            entity
        );
        animator.Initialize(anim);
        sprite.gameObject.SetActive(true);
        sprite.color = card.color;

        // Start action timer coroutine
        StartCoroutine(ActionLoop());
    }


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

    public override HashSet<GameObject> GetActionTargets(GameObject location)
    {
        // location is the upper left grid cell our creature is in

        //turn locations into targets
        HashSet<GameObject> targets = new HashSet<GameObject>();
        //foreach (GameObject g in locations)
        //{
        //    if (g.GetComponent<CardManager>() != null ||
        //        g.GetComponent<Entity>() != null)
        //    {
        //        targets.Add(g);
        //    }
        //    else if (g.GetComponent<GridCell>() != null &&
        //             g.GetComponent<GridCell>().GetCreature() != null)
        //    {
        //        targets.Add(g.GetComponent<GridCell>().GetCreature().gameObject);
        //    }
        //}

        return targets;
    }

    private IEnumerator ActionLoop()
    {
        while (entity.Health > 0)
        {
            if (actionTimer.IsComplete())
            {
                // Raise event to let entity know
                entity.TriggerActionEvent();

                if (Random.Range(0, 100) > card.actionChance)
                {
                    Debug.Log(gameObject.name + " action failed");
                }
                else
                {
                    HashSet<GameObject> targets = GetActionTargets(grid.GetCell(coordinates).gameObject);
                    targets = FilterTargetsByCondition(targets);
                    ExecuteActionOnTargets(targets);

                    actionTimer.StartTimer(creatureCard.actionTime);
                }

            }
            yield return null;
        }

        // when health <= 0
        DestroySelf();
    }

    // added onto event OnHealthChange of entity
    public void SetHealthDisplay(int oldHP, int newHP)
    {
        gridHealthText.text = newHP.ToString();
    }

    // added onto event OnSpeedChange of entity
    public void SetSpeedDisplay(float s)
    {
        // if speed is ~1f
        if (Mathf.Abs(s - 1) < Mathf.Epsilon)
        {
            gridSpeedText.text = "";
        }
        else
        {
            gridSpeedText.text = "x" + (Mathf.Round(s * 10) / 10f).ToString();
        }
    }
}

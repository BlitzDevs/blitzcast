using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all of the displays and events that a card object does.
/// Inherits from CardManager and deals with display/cast/action specific to
/// SpellCards.
/// The card itself is determined by Card.
/// </summary>
/// <seealso cref="Card"/>
/// <seealso cref="CardManager"/>
/// <seealso cref="CreatureCardManager"/>
public class SpellCardManager: CardManager
{

    [SerializeField] protected TrailRenderer trail;

    private SpellCard spellCard;


    /// <summary>
    /// Initialize this CardManager; set values and initiate displays.
    /// Also initiate the trail, which is specific to Spells.
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

        spellCard = (SpellCard)card;
        trail.startColor = card.color;
        trail.endColor = new Color(card.color.r, card.color.g, card.color.b, 0f);
    }

    /// <summary>
    /// Called every frame inside OnDrag(); for showing cast location through
    /// cell highlighting.
    /// </summary>
    public override void TryPreview()
    {
        GameObject target = GetCastLocation();
        // if the current mouse position points to a valid cast target position...
        if (target != null)
        {
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
    }

    /// <summary>
    /// Get the GameObject of a valid cast location based on where the
    /// mouse cursor currently is.
    /// For Spells, depends on the SpellTarget.
    /// </summary>
    /// <returns>
    /// The GameObject of a valid cast location; otherwise, null.
    /// </returns>
    public override GameObject GetCastLocation()
    {
        switch (spellCard.targetArea)
        {
            case SpellCard.SpellTarget.Single:
                switch (spellCard.action)
                {
                    case Card.Action.None:
                    case Card.Action.Damage:
                    case Card.Action.Heal:
                        // Target Creature or Player
                        GameObject cellObject = gameManager.GetFirstUnderCursor<GridCell>();
                        if (cellObject != null)
                        {
                            return cellObject;
                        }
                        return gameManager.GetFirstUnderCursor<PlayerManager>();

                    case Card.Action.Counter:
                        // Target Casting Cards
                        int targetLayer = SortingLayer
                            .GetLayerValueFromName("Casting");
                        return gameManager.GetFirstUnderCursor(targetLayer);

                    case Card.Action.Destroy:
                        return gameManager.GetFirstUnderCursor<GridCell>();
                }
                break;

            // When target area is a shape (not single), target is Creature Grid
            case SpellCard.SpellTarget.Cross:
            case SpellCard.SpellTarget.Square:
            case SpellCard.SpellTarget.Row:
            case SpellCard.SpellTarget.Column:
            case SpellCard.SpellTarget.SingleCreature:
                return gameManager.GetFirstUnderCursor<GridCell>();

            case SpellCard.SpellTarget.AllCreatures:
            case SpellCard.SpellTarget.All:
                int castAllZoneLayer = LayerMask.NameToLayer("Cast All Zone");
                return gameManager.GetFirstUnderCursor(castAllZoneLayer);

            default:
                Debug.LogWarning("Card Target Area is undefined");
                break;
        }

        return null;
    }

    /// <summary>
    /// Get the targets for the cast based on the location of the cast and the
    /// spell target shape/type.
    /// </summary>
    /// <returns>
    /// List of GameObjects containing the target GameObjects.
    /// </returns>
    public override List<GameObject> GetCastTargets(GameObject locationObject) {
        List<GameObject> targets = new List<GameObject>();
        GridCell cell = locationObject.GetComponent<GridCell>();

        switch (spellCard.targetArea)
        {
            case SpellCard.SpellTarget.Single:
                if (locationObject.GetComponent<Entity>() != null)
                {
                    targets.Add(locationObject);
                }
                else goto case SpellCard.SpellTarget.SingleCreature;
                break;

            case SpellCard.SpellTarget.SingleCreature:
                if (cell != null)
                {
                    targets.Add(cell.gameObject);
                }
                break;

            case SpellCard.SpellTarget.Cross:
                if (cell != null)
                {
                    Vector2Int location = new
                        Vector2Int(cell.coordinates.x, cell.coordinates.y);

                    // drc stands for Delta Row Column (blame dj)
                    List<Vector2Int> drc = new List<Vector2Int>();
                    drc.Add(Vector2Int.zero);
                    drc.Add(Vector2Int.down);
                    drc.Add(Vector2Int.up);
                    drc.Add(Vector2Int.left);
                    drc.Add(Vector2Int.right);

                    foreach (Vector2Int direction in drc)
                    {
                        GridCell drcCell = grid.GetCell(location + direction);
                        if (drcCell != null)
                        {
                            targets.Add(drcCell.gameObject);
                        }
                    }
                }
                break;

            case SpellCard.SpellTarget.Square:
                if (cell != null)
                {
                    Vector2Int location = new
                        Vector2Int(cell.coordinates.x, cell.coordinates.y);

                    // dr/c stands for Delta Row/Column
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            Vector2Int drc = new Vector2Int(dr, dc);

                            GridCell drcCell = grid.GetCell(location + drc);
                            if (drcCell != null)
                            {
                                targets.Add(drcCell.gameObject);
                            }
                        }
                }
                break;

            case SpellCard.SpellTarget.Row:
                if (cell != null)
                {
                    for (int c = 0; c < grid.size.y; c++)
                    {
                        Vector2Int rc = new Vector2Int(cell.coordinates.x, c);
                        GridCell drcCell = grid.GetCell(rc);
                        if (drcCell != null)
                        {
                            targets.Add(drcCell.gameObject);
                        }
                    }
                }
                break;

            case SpellCard.SpellTarget.Column:
                if (cell != null)
                {
                    for (int r = 0; r < grid.size.x; r++)
                    {
                        Vector2Int rc = new Vector2Int(r, cell.coordinates.y);
                        GridCell drcCell = grid.GetCell(rc);
                        if (drcCell != null)
                        {
                            targets.Add(drcCell.gameObject);
                        }
                    }
                }
                break;

            case SpellCard.SpellTarget.All:
                targets.Add(gameManager.playerA.gameObject);
                targets.Add(gameManager.playerB.gameObject);
                goto case SpellCard.SpellTarget.AllCreatures;

            case SpellCard.SpellTarget.AllCreatures:
                foreach (GridCell c in grid.cells)
                {
                    targets.Add(c.gameObject);
                }
                break;

            default:
                Debug.LogWarning("Area not implemented");
                break;
        }

        return targets;
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
        // GetCastTargets adds targets to our list based on the TargetArea
        List<GameObject> locations = GetCastTargets(locationObject);

        //turn locations into targets
        HashSet<GameObject> targets = new HashSet<GameObject>();
        foreach (GameObject g in locations)
        {
            if (g.GetComponent<CardManager>() != null ||
                g.GetComponent<Entity>() != null)
            {
                targets.Add(g);
            }
            else if (g.GetComponent<GridCell>() != null &&
                     g.GetComponent<GridCell>().GetCreature() != null)
            {
                targets.Add(g.GetComponent<GridCell>().GetCreature().gameObject);
            }
        }

        return targets;
    }

    /// <summary>
    /// Cast this card onto a valid location (which should have been determined
    /// by GetCastLocation()).
    /// For Spell, execute the action on all the spell targets.
    /// </summary>
    public override void Cast(GameObject locationObject)
    {
        // (Mochel) First off, let's see if this boi even lands a hit
        if (Random.Range(0, 100) > spellCard.actionChance) {
            DestroySelf();
            Debug.Log(gameObject.name + " action failed, object destroyed");
            return;
        }

        HashSet<GameObject> targets = GetActionTargets(locationObject);
        targets = FilterTargetsByCondition(targets);
        ExecuteActionOnTargets(targets);

        DestroySelf();
    }


}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SpellCardManager: CardManager
{

    [SerializeField] protected TrailRenderer trail;
    [SerializeField] protected TMP_Text descriptionText;

    private SpellCard spellCard;


    public override void Initialize(Card card, HandSlot slot, PlayerManager player)
    {
        base.Initialize(card, slot, player);

        spellCard = (SpellCard)card;
        descriptionText.text = card.description;
        trail.startColor = card.color;
        trail.endColor = new Color(card.color.r, card.color.g, card.color.b, 0f);
    }

    public override void TryPreview()
    {
        GameObject target = GetCastLocation();
        if (target != null)
        {
            //snap to target
            //sprite.transform.position = target.transform.position;
            spriteMover.SetPosition(target.transform.position);

            foreach (GameObject targetObject in GetCastTargets(target))
            {
                Highlightable highlightable = targetObject.gameObject.GetComponent<Highlightable>();
                previewHighlightables.Add(highlightable);
                highlightable.Highlight(card.color);
            }
        }
    }


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

    //returns HashSet of GridCell Objects or Player Object
    public override List<GameObject> GetCastTargets(GameObject target) {
        List<GameObject> targets = new List<GameObject>();
        GridCell cell = target.GetComponent<GridCell>();

        switch (spellCard.targetArea)
        {
            case SpellCard.SpellTarget.Single:
                if (target.GetComponent<Entity>() != null)
                {
                    targets.Add(target);
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

    public override HashSet<GameObject> GetActionTargets(GameObject location)
    {
        // GetCastTargets adds targets to our list based on the TargetArea
        List<GameObject> locations = GetCastTargets(location);

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

    public override void Cast(GameObject location)
    {
        // First off, let's see if this boi even lands a hit
        if (Random.Range(0, 100) > spellCard.actionChance) {
            DestroySelf();
            Debug.Log(gameObject.name + " action failed, object destroyed");
            return;
        }

        HashSet<GameObject> targets = GetActionTargets(location);
        targets = FilterTargetsByCondition(targets);
        ExecuteActionOnTargets(targets);

        DestroySelf();
    }


}

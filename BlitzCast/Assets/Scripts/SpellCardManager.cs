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
            sprite.transform.position = target.transform.position;

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
        switch (spellCard.cardBehavior.targetArea)
        {
            case Card.TargetArea.Single:
                switch (spellCard.cardBehavior.action)
                {
                    case Card.Action.Damage:
                    case Card.Action.Heal:
                    case Card.Action.IncreaseHP:
                        // Target Creature or Player
                        GameObject cellObject = gameManager
                            .GetFirstUnderCursor<GridCell>();
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
            case Card.TargetArea.Cross:
            case Card.TargetArea.Square:
            case Card.TargetArea.Row:
            case Card.TargetArea.Column:
            case Card.TargetArea.SingleCreature:
                return gameManager.GetFirstUnderCursor<GridCell>();

            case Card.TargetArea.AllCreatures:
            case Card.TargetArea.All:
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

        switch (spellCard.cardBehavior.targetArea)
        {
            case Card.TargetArea.Single:
                if (target.GetComponent<Entity>() != null)
                {
                    targets.Add(target);
                }
                else goto case Card.TargetArea.SingleCreature;
                break;

            case Card.TargetArea.SingleCreature:
                if (cell != null)
                {
                    targets.Add(cell.gameObject);
                }
                break;

            case Card.TargetArea.Cross:
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

            case Card.TargetArea.Square:
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

            case Card.TargetArea.Row:
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

            case Card.TargetArea.Column:
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

            case Card.TargetArea.All:
                targets.Add(gameManager.playerA.gameObject);
                //targets.Add(gameManager.playerB.gameObject);
                goto case Card.TargetArea.AllCreatures;

            case Card.TargetArea.AllCreatures:
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

    public override void Cast(GameObject location)
    {
        // First off, let's see if this boi even lands a hit
        if (Random.Range(0.0f, 1.0f) > spellCard.actionChance) {
            DestroySelf();
            Debug.Log(gameObject.name + " action failed, object destroyed");
            return;
        }

        // GetCastTargets adds targets to our list based on the TargetArea
        List<GameObject> locations = GetCastTargets(location);

        //turn locations into targets
        HashSet<GameObject> targets = new HashSet<GameObject>();
        foreach(GameObject g in locations)
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

        // Filter targets based on conditions
        HashSet<GameObject> targetsCopy = new HashSet<GameObject>(targets);

        foreach (GameObject t in targetsCopy)
        {
            bool valid = true;

            Entity tEntity = t.GetComponent<Entity>();
            CardManager tCard = t.GetComponent<CardManager>();

            switch (spellCard.condition)
            {
                case SpellCard.Condition.None:
                    break;

                case SpellCard.Condition.HPGreaterThan:
                    if (tEntity != null)
                    {
                        valid = tEntity.Health > spellCard.conditionValue;
                    }
                    break;

                case SpellCard.Condition.HPLessThan:
                    if (tEntity != null)
                    {
                        valid = tEntity.Health < spellCard.conditionValue;
                    }
                    break;

                case SpellCard.Condition.Race:

                    if (tCard != null)
                    {
                        valid = tCard.card.race ==
                            (Card.Race)spellCard.conditionValue;
                    }
                    break;

                case SpellCard.Condition.Friendly:
                    if (tCard != null)
                    {
                        valid = tCard.team == GameManager.Team.Friendly;
                    }
                    break;

                case SpellCard.Condition.Enemy:
                    if (tCard != null)
                    {
                        valid = tCard.team == GameManager.Team.Enemy;
                    }
                    break;

                case SpellCard.Condition.Status:
                    valid = false;
                    foreach (Entity.Status s in tEntity.statuses)
                    {
                        if ((int)s.statusType == spellCard.conditionValue)
                        {
                            valid = true;
                        }
                    }
                    break;

                default:
                    Debug.LogWarning("Condition not implemented");
                    break;
            }

            // Remove target if marked invalid
            if (!valid)
            {
                targets.Remove(t);
            }
        } // end Conditions foreach loop

        // Execute Card Action on our list of valid targets
        foreach (GameObject t in targets)
        {
            Entity tEntity = t.GetComponent<Entity>();
            CardManager tCard = t.GetComponent<CardManager>();

            switch (spellCard.cardBehavior.action)
            {
                case Card.Action.Damage:
                    if (tEntity != null)
                    tEntity.Health -= card.cardBehavior.actionValue;
                    break;

                case Card.Action.Heal:
                    tEntity.Health += card.cardBehavior.actionValue;
                    break;

                case Card.Action.IncreaseHP:
                    tEntity.MaxHealth += card.cardBehavior.actionValue;
                    break;

                case Card.Action.Destroy:
                    tCard.DestroySelf();
                    break;

                default:
                    Debug.LogWarning("Card Action not implemented");
                    break;
            }

            tEntity.ApplyStatus(
                card.cardBehavior.statusInflicted,
                card.cardBehavior.stacks
            );
        }

        DestroySelf();
    }


}

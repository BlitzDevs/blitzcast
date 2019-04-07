using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SpellCardManager: CardManager
{

    [SerializeField] protected TrailRenderer trail;
    [SerializeField] protected TMP_Text descriptionText;

    private SpellCard spellCard;


    public override void Initialize(
        Card card, GameManager.Team team, HandSlot slot)
    {
        base.Initialize(card, team, slot);

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
                Image image = targetObject.gameObject.GetComponent<Image>();
                previewImages.Add(image);
                image.color = card.color;
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
                return gameManager.GetFirstUnderCursor<GridCell>();

            case Card.TargetArea.AllCreatures:
            case Card.TargetArea.All:
                int castAllZoneLayer = SortingLayer
                    .GetLayerValueFromName("Cast All Zone");
                return gameManager.GetFirstUnderCursor(castAllZoneLayer);

            default:
                Debug.LogWarning("Card Target Area is undefined");
                break;
        }

        return null;
    }

    //returns HashSet of GridCell Objects or Player Object
    public override HashSet<GameObject> GetCastTargets(GameObject target) {
        HashSet<GameObject> targets = new HashSet<GameObject>();
        GridCell cell = target.GetComponent<GridCell>();

        switch (spellCard.cardBehavior.targetArea)
        {
            case Card.TargetArea.Single:
                if (target.GetComponent<PlayerManager>() != null)
                {
                    targets.Add(target);
                }
                else goto case Card.TargetArea.SingleCreature;
                break;

            case Card.TargetArea.SingleCreature:
                if (cell != null)
                {
                    //CreatureCardManager targetCreature = grid
                    //    .GetCreature(cell.coordinates);
                    //if (targetCreature != null)
                    //{
                    //    //targets.Add(targetCreature.gameObject);
                    //}

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
                        //CreatureCardManager targetCreature = grid
                        //    .GetCreature(location + direction);
                        //if (targetCreature != null)
                        //{
                        //    //targets.Add(targetCreature.gameObject);
                        //}
                        GridCell drcCell = grid.GetCellRC(location + direction);
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

                            //CreatureCardManager targetCreature = grid
                            //    .GetCreature(location + drc);

                            //if (targetCreature != null)
                            //{
                            //    //targets.Add(targetCreature.gameObject);
                            //}

                            GridCell drcCell = grid.GetCellRC(location + drc);
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
                        GridCell drcCell = grid.GetCellRC(rc);
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
                        GridCell drcCell = grid.GetCellRC(rc);
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

    public override void Cast(GameObject target)
    {
        // GetCastTargets adds targets to our list based on the TargetArea
        HashSet<GameObject> targets = GetCastTargets(target);

        // Filter targets based on conditions
        HashSet<GameObject> targetsCopy = new HashSet<GameObject>(targets);
        foreach (GameObject t in targetsCopy)
        {
            bool valid = true;

            GridCell tCell = t.GetComponent<GridCell>();
            IEntity entity = tCell != null ?
                grid.GetCreature(tCell.coordinates) :
                t.GetComponent<IEntity>();
            if (entity == null)
            {
                valid = false;
            }

            switch (spellCard.condition)
            {
                case SpellCard.Condition.None:
                    break;

                case SpellCard.Condition.HPGreaterThan:
                    valid = entity.GetHealth() > spellCard.conditionValue;
                    break;

                case SpellCard.Condition.HPLessThan:
                    valid = entity.GetHealth() < spellCard.conditionValue;
                    break;

                case SpellCard.Condition.Race:
                    valid = t.GetComponent<CardManager>().card.race ==
                        (Card.Race) spellCard.conditionValue;
                    break;

                case SpellCard.Condition.Friendly:
                    valid = t.GetComponent<CardManager>().team ==
                        GameManager.Team.Friendly;
                    break;

                case SpellCard.Condition.Enemy:
                    valid = t.GetComponent<CardManager>().team ==
                        GameManager.Team.Enemy;
                    break;

                case SpellCard.Condition.Status:
                    valid = false;
                    foreach (Status s in t.GetComponent<CreatureCardManager>().statuses)
                    {
                        if ((int) s.statusType == spellCard.conditionValue)
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
            GridCell tCell = t.GetComponent<GridCell>();
            IEntity entity = tCell != null ?
                grid.GetCreature(tCell.coordinates) :
                t.GetComponent<IEntity>();

            switch (spellCard.cardBehavior.action)
            {
                case Card.Action.Damage:
                    entity.Damage(card.cardBehavior.actionValue);
                    break;

                case Card.Action.Heal:
                    entity.Heal(card.cardBehavior.actionValue);
                    break;

                case Card.Action.Destroy:
                    CreatureCardManager creatureTarget =
                        t.GetComponent<CreatureCardManager>();
                    if (creatureTarget != null)
                    {
                        creatureTarget.DestroySelf();
                    }
                    else
                    {
                        CardManager cardTarget = t.GetComponent<CardManager>();
                        if (cardTarget != null)
                        {
                            cardTarget.DestroySelf();
                        }
                    }
                    break;

                default:
                    Debug.LogWarning("Card Action not implemented");
                    break;
            }

            entity.ApplyStatus(
                spellCard.cardBehavior.statusInflicted,
                spellCard.cardBehavior.stacks
            );
        }

        DestroySelf();
    }


}

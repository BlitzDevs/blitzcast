using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SpellCardManager: CardManager
{
    [SerializeField] protected TMP_Text descriptionText;


    public override void Initialize(Card card, GameManager.Team team, HandSlot slot)
    {
        this.card = card;
        this.team = team;
        this.slot = slot;

        descriptionText.text = card.description;
    }

    public override void Cast(GameObject target) {
        SpellCard spellCard = (SpellCard) card;
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
                    Debug.Log("Adding Creature to targets");
                     
                    CreatureCardManager targetCreature = grid.GetCreature(cell.coordinates);
                    if (targetCreature != null)
                    {
                        targets.Add(targetCreature.gameObject);
                    }
                }
                break;
            case Card.TargetArea.Cross:
                if (cell != null)
                {
                    Vector2Int location = new Vector2Int(cell.coordinates.x, cell.coordinates.y);
                    //DeltaRowColumn 
                    List<Vector2Int> drc = new List<Vector2Int>();
                    drc.Add(Vector2Int.down);
                    drc.Add(Vector2Int.up);
                    drc.Add(Vector2Int.left);
                    drc.Add(Vector2Int.right);

                    foreach (Vector2Int direction in drc)
                    {
                        CreatureCardManager targetCreature = grid.GetCreature(location + direction);
                        if (targetCreature != null)
                        {
                            targets.Add(targetCreature.gameObject);
                        }
                    }
                }
                break;
            case Card.TargetArea.Square:
                if (cell != null)
                {
                    Vector2Int location = new Vector2Int(cell.coordinates.x, cell.coordinates.y);
                    //DeltaRowColumn 
                    for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        Vector2Int drc = new Vector2Int(dr, dc);
                        CreatureCardManager targetCreature = grid.GetCreature(location + drc);
                        if (targetCreature != null)
                        {
                            targets.Add(targetCreature.gameObject);
                        }
                    }
                }
                break;
            case Card.TargetArea.Row:
                if (cell != null)
                {
                    for(int c = 0; c < grid.size.y; c++)
                    {
                        CreatureCardManager targetCreature = grid.GetCreature(new Vector2Int(cell.coordinates.x, c));
                        if (targetCreature != null)
                        {
                            targets.Add(targetCreature.gameObject);
                        }
                    }
                }
                break;
            case Card.TargetArea.Column:
                if (cell != null)
                {
                    for (int r = 0; r < grid.size.x; r++)
                    {
                        CreatureCardManager targetCreature = grid.GetCreature(new Vector2Int(r, cell.coordinates.y));
                        if (targetCreature != null)
                        {
                            targets.Add(targetCreature.gameObject);
                        }
                    }
                }
                break;
            case Card.TargetArea.All:
                targets.Add(gameManager.playerA.gameObject);
                //targets.Add(gameManager.playerB.gameObject);
                goto case Card.TargetArea.AllCreatures;
            case Card.TargetArea.AllCreatures:
                foreach (CreatureCardManager c in grid.GetAllCreatures())
                {
                    targets.Add(c.gameObject);
                }
                break;
            default:
                Debug.LogWarning("Area not implemented");
                break;
        }


        //Filter conditions
        HashSet<GameObject> targetsCopy = new HashSet<GameObject>(targets);
        foreach (GameObject t in targetsCopy)
        {
            IEntity damageableTarget = t.GetComponent<IEntity>();

            bool valid = true;
            switch (spellCard.cardBehavior.condition)
            {
                case Card.Condition.None:
                    break;
                case Card.Condition.HPGreaterThan:
                    valid = damageableTarget.GetHealth() > card.cardBehavior.conditionValue;
                    break;
                case Card.Condition.HPLessThan:
                    valid = damageableTarget.GetHealth() < card.cardBehavior.conditionValue;
                    break;
                case Card.Condition.Race:
                    valid = t.GetComponent<CardManager>().card.race == (Card.Race)card.cardBehavior.conditionValue;
                    break;
                case Card.Condition.Friendly:
                    valid = t.GetComponent<CardManager>().team == GameManager.Team.A;
                    break;
                case Card.Condition.Enemy:
                    valid = t.GetComponent<CardManager>().team == GameManager.Team.B;
                    break;
                case Card.Condition.Status:
                    valid = t.GetComponent<CreatureCardManager>().statuses.Contains((Card.Status)card.cardBehavior.conditionValue);
                    break;
                default:
                    Debug.LogWarning("Condition not implemented");
                    break;
            }

            if (!valid)
            {
                targets.Remove(t);
            }
        }
            // Loop through targets
        foreach (GameObject t in targets)
        {
            Debug.Log(t.name);
            IEntity damageableTarget = t.GetComponent<IEntity>();
            switch (spellCard.cardBehavior.action)
            {
                case Card.Action.Damage:
                    damageableTarget.Damage(card.cardBehavior.actionValue);
                    break;
                case Card.Action.Heal:
                    damageableTarget.Heal(card.cardBehavior.actionValue);
                    break;
                case Card.Action.Destroy:
                    CreatureCardManager creatureTarget = t.GetComponent<CreatureCardManager>();
                    creatureTarget.DestroySelf();
                    break;
                default:
                    CardManager cardTarget = t.GetComponent<CardManager>();
                    cardTarget.DestroySelf();
                    break;
            }
        }
        DestroySelf();
    }


    public override void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public override void EnablePreview(GameObject target)
    {
        SpellCard spellCard = (SpellCard) card;

    }
    public override void DisablePreview()
    {
        Debug.Log("Disable Preview");
    }

    public override GameObject GetCastLocation()
    {
        SpellCard spellCard = (SpellCard) card;
        switch (spellCard.cardBehavior.targetArea)
        {
            case Card.TargetArea.Single:
                switch (spellCard.cardBehavior.action)
                {
                    case Card.Action.Damage:
                    case Card.Action.Heal:
                        // Target Creature or Player
                        GameObject creatureCell = gameManager.GetFirstUnderCursor<GridCell>();
                        if (creatureCell != null)
                        {
                            return creatureCell;
                        }
                        else
                        {
                            return gameManager.GetFirstUnderCursor<PlayerManager>();
                        }
                    case Card.Action.Counter:
                        // Target Casting Cards
                        int targetLayer = SortingLayer.GetLayerValueFromName("Casting");
                        return gameManager.GetFirstUnderCursor(targetLayer);
                    case Card.Action.Destroy:
                        return gameManager.GetFirstUnderCursor<GridCell>();
                }
                break;
            case Card.TargetArea.Cross:
            case Card.TargetArea.Square:
            case Card.TargetArea.Row:
            case Card.TargetArea.Column:
                // When target area is a shape (not single), target is Creature Grid
                return gameManager.GetFirstUnderCursor<GridCell>();
            case Card.TargetArea.AllCreatures:
            case Card.TargetArea.All:
                int castAllZoneLayer = SortingLayer.GetLayerValueFromName("Cast All Zone");
                return gameManager.GetFirstUnderCursor(castAllZoneLayer);
            default:
                Debug.LogWarning("Card Target Area is undefined");
                break;
        }

        return null;
    }


}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SpellCardManager: CardManager
{

    [SerializeField] protected TMP_Text descriptionText;
    private SpellCard spellCard;


    public override void Initialize(
        Card card, GameManager.Team team, HandSlot slot)
    {
        base.Initialize(card, team, slot);

        spellCard = (SpellCard)card;

        descriptionText.text = card.description;
    }

    public override void EnablePreview()
    {
        GameObject target = GetCastLocation();

        //disable card display
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        //enable sprite
        sprite.gameObject.SetActive(true);
        //snap to target
        gameObject.transform.position = target.transform.position;

        foreach (GameObject targetObject in GetCastTargets(target))
        {
            Image image = targetObject.gameObject.GetComponent<Image>();
            image.color = Color.yellow;
        }
    }

    public override void DisablePreview()
    {
        //enable card display
        cardFront.SetActive(true);
        cardBack.SetActive(false);
        //disable sprite
        sprite.gameObject.SetActive(false);
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
                        GameObject creatureCell = gameManager
                            .GetFirstUnderCursor<GridCell>();
                        if (creatureCell != null)
                        {
                            return creatureCell;
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
                    Debug.Log("Adding Creature to targets");

                    CreatureCardManager targetCreature = grid
                        .GetCreature(cell.coordinates);
                    if (targetCreature != null)
                    {
                        targets.Add(targetCreature.gameObject);
                    }
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
                        CreatureCardManager targetCreature = grid
                            .GetCreature(location + direction);
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
                    Vector2Int location = new
                        Vector2Int(cell.coordinates.x, cell.coordinates.y);

                    // dr/c stands for Delta Row/Column 
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            Vector2Int drc = new Vector2Int(dr, dc);
                            CreatureCardManager targetCreature = grid
                                .GetCreature(location + drc);

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
                    for (int c = 0; c < grid.size.y; c++)
                    {
                        CreatureCardManager targetCreature = grid.GetCreature(
                            new Vector2Int(cell.coordinates.x, c));
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
                        CreatureCardManager targetCreature = grid.GetCreature(
                            new Vector2Int(r, cell.coordinates.y));
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

        return targets;
    }

    public override void Cast(GameObject target) {
        HashSet<GameObject> targets = GetCastTargets(target);
        // Add targets to our list based on the TargetArea
        

        // Filter targets based on conditions
        HashSet<GameObject> targetsCopy = new HashSet<GameObject>(targets);
        foreach (GameObject t in targetsCopy)
        {
            IEntity entity = t.GetComponent<IEntity>();

            bool valid = true;
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
        }

        // Execute Card Action on our list of valid targets
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
                    CreatureCardManager creatureTarget =
                        t.GetComponent<CreatureCardManager>();
                    creatureTarget.DestroySelf();
                    break;
                default:
                    CardManager cardTarget = t.GetComponent<CardManager>();
                    cardTarget.DestroySelf();
                    break;
            }

            damageableTarget.ApplyStatus(spellCard.cardBehavior.statusInflicted, spellCard.cardBehavior.stacks);
        }

        DestroySelf();
    }

    public override void DestroySelf()
    {
        Destroy(this.gameObject);
    }

}

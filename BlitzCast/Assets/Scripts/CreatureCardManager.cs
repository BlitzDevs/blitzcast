using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class CreatureCardManager : CardManager, IEntity
{

    public List<Status> statuses;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text actionValueText;
    [SerializeField] private TMP_Text actionTimeText;

    [SerializeField] private GameObject gridDisplayObject;
    [SerializeField] private GameObject gridStatusesParent;
    [SerializeField] private TMP_Text gridHealthText;
    [SerializeField] private TMP_Text gridActionValueText;
    [SerializeField] private CircleTimer actionTimer;

    private int health;
    private int maxHealth;
    private int frameDamage;
    private int actionValue;
    private int actionTime;

    private Vector2Int location;
    private CreatureCard creatureCard;

    private Vector2 spriteSize;
    private Vector3 sizeOffset;


    // Initialize is our own function which is called by HandSlot
    public override void Initialize(
        Card card, GameManager.Team team, HandSlot slot)
    {
        base.Initialize(card, team, slot);

        statuses = new List<Status>();
        creatureCard = (CreatureCard) card;

        health = creatureCard.health;
        maxHealth = creatureCard.health;
        frameDamage = 0;
        actionValue = creatureCard.cardBehavior.actionValue;
        actionTime = creatureCard.actionTime;
        healthText.text = health.ToString();
        actionValueText.text = actionValue.ToString();
        actionTimeText.text = actionTime.ToString();

        spriteSize = new Vector2(
            sprite.rectTransform.rect.width * creatureCard.size.x,
            sprite.rectTransform.rect.height * creatureCard.size.y
        );
        sizeOffset = new Vector3(
            sprite.rectTransform.rect.width * (creatureCard.size.x - 1f) / 2f,
            -sprite.rectTransform.rect.height * (creatureCard.size.y - 1f) / 2f,
            0f
        );
        sprite.rectTransform.sizeDelta = spriteSize;

        gridDisplayObject.SetActive(false);
    }

    public override void TryPreview()
    {
        GameObject target = GetCastLocation();
        if (target != null)
        {
            //set color/transparency
            sprite.color = new Color(0, 0, 0, 0.5f);
            //snap to target
            //sprite.transform.position = target.transform.position + sizeOffset;
            sprite.transform.position = target.transform.position;
            sprite.transform.localPosition += sizeOffset;

            foreach (GameObject targetObject in GetCastTargets(target))
            {
                Image image = targetObject.gameObject.GetComponent<Image>();
                previewImages.Add(image);
                image.color = card.color;
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

        if (cell != null &&
            cell.coordinates.x >= grid.size.x / 2 &&
            cell.coordinates.y + creatureCard.size.y - 1 < grid.size.y &&
            cell.coordinates.x + creatureCard.size.x - 1 < grid.size.x)
        {
            for (int r = 0; r < creatureCard.size.x; r++)
            for (int c = 0; c < creatureCard.size.y; c++)
            {
                Vector2Int rc = new Vector2Int(
                    cell.coordinates.x + r, cell.coordinates.y + c);
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
            targets.Add(grid.GetCellRC(rc).gameObject);
        }
        return targets;
    }

    public override void Cast(GameObject target)
    {
        GridCell cell = target.GetComponent<GridCell>();
        location = cell.coordinates;

        gameObject.layer = SortingLayer.NameToID("Creatures");

        // add creature location to grid
        foreach (GameObject cellObject in GetCastTargets(target))
        {
            GridCell c = cellObject.GetComponent<GridCell>();
            grid.creatures.Add(c.coordinates, this);
        }

        // Turn CreatureCard into Creature on grid
        gameObject.name = card.cardName;
        // move out of the hierarchy
        sprite.transform.SetParent(transform);
        sprite.transform.localPosition = Vector3.zero + sizeOffset;
        transform.SetParent(grid.playerCreaturesParent);
        // move onto grid position
        transform.position = target.transform.position;

        // Enable Grid Creature Display
        gridDisplayObject.SetActive(true);
        RectTransform rt = gridDisplayObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(
            rt.rect.width * creatureCard.size.x,
            rt.rect.height * creatureCard.size.y
        );
        gridDisplayObject.transform.localPosition = Vector3.zero + sizeOffset;
        gridHealthText.text = health.ToString();
        gridActionValueText.text = actionValue.ToString();
        actionTimer.StartTimer(actionTime);

        // disable card display
        SetTint(new Color(0f, 0f, 0f, 0f));
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        // enable sprite display
        sprite.gameObject.SetActive(true);
        // set color/transparency to normal
        sprite.color = card.color;


        // Start action timer coroutine
        StartCoroutine(DoAction());
    }


    public override void DestroySelf()
    {
        //delete creature locations from CreatureGrid
        for (int r = 0; r < creatureCard.size.x; r++)
            for (int c = 0; c < creatureCard.size.y; c++)
            {
                Vector2Int rc = new Vector2Int(location.x + r, location.y + c);
                grid.creatures.Remove(rc);
            }

        base.DestroySelf();
    }

    private IEnumerator DoAction()
    {
        while (health > 0)
        {
            DoStatuses();
            if (actionTimer.IsComplete())
            {
                switch (card.cardBehavior.action)
                {
                    case Card.Action.Damage:
                        Debug.Log(card.cardName + " deal damage");
                        break;
                    case Card.Action.Heal:
                        Debug.Log(card.cardName + " heals");
                        break;
                    case Card.Action.Destroy:
                        Debug.Log(card.cardName + " destroys");
                        break;
                    default:
                        break;
                }
                actionTimer.StartTimer(creatureCard.actionTime);
            }
            yield return null;
        }

        // when health <= 0
        DestroySelf();
    }

    public void DoStatuses()
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            // Careful! Structs are immutable types in C#,
            // so have to make a new Status when changing a value.
            Status status = statuses[i];

            switch (statuses[i].statusType)
            {
                case Card.StatusType.Frozen:
                    // stop timer from progressing
                    actionTimer.countdown += Time.deltaTime;
                    // after 1 second, remove 1 stack
                    if (gameManager.timer.elapsedTime - statuses[i].startTime > 1f)
                    {
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            gameManager.timer.elapsedTime
                        );
                    }
                    break;

                case Card.StatusType.Bleeding:
                    // after 1 second, deal (stacks) damage and remove 1 stack
                    if (gameManager.timer.elapsedTime - statuses[i].startTime > 1f)
                    {
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            gameManager.timer.elapsedTime
                        );
                        Damage(status.stacks);
                    }
                    break;
/* TODO: prevent stackoverflow
                case Card.StatusType.Shielded:
                    if (frameDamage > 0)
                    {
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            status.startTime
                        );
                        frameDamage = 0;
                    }
                    break;
*/
                case Card.StatusType.Wounded:
                    if (actionTimer.IsComplete())
                    {
                        //we don't care about startTime for wounded
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            status.startTime
                        );
                        Damage(status.stacks);
                    }
                    break;
                default:
                    Debug.LogWarning("Status not implemented");
                    break;   
            }

            if (status.stacks == 0)
            {
                statuses.RemoveAt(i);
                i--;
            }
        }
    }

    public void Damage(int hp)
    {
        frameDamage = hp;
        //DoStatuses();
        SetHealth(health - frameDamage);
    }

    public void Heal(int hp)
    {
        SetHealth(Math.Min(health + hp, maxHealth));
    }

    public void IncreaseHP (int hp)
    {
        maxHealth += hp;
        SetHealth(health + hp);
    }


    private void SetHealth(int hp)
    {
        health = hp;

        // change health display
        gridHealthText.text = health.ToString();
    }

    public int GetHealth()
    {
        return health;
    }

    public List<Status> GetStatuses()
    {
        return statuses;
    }

    public void ApplyStatus(Card.StatusType statusType, int stacks)
    {
        //GameObject statusObject = new
        //statusObject.transform.SetParent(gridStatusesParent);

        // if status already exists, add stacks
        for (int i = 0; i < statuses.Count; i++)
        {
            Status s = statuses[i];
            if (s.statusType == statusType)
            {
                s.stacks += stacks;
                return;
            }
        }
        // otherwise add
        statuses.Add(new Status(statusType, stacks, gameManager.timer.elapsedTime));
    }
}
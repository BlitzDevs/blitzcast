using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CreatureCardManager : CardManager, IEntity
{

    public List<Status> statuses;

    public TMP_Text healthText;
    public TMP_Text actionValueText;
    public TMP_Text actionTimeText;

    public GameObject gridDisplayObject;
    public GameObject gridStatusesParent;
    public TMP_Text gridHealthText;
    public TMP_Text gridActionValueText;
    public TMP_Text gridActionTimeText;
    public Image gridActionTimer;

    private int health;
    private int actionValue;
    private int actionTime;
    private float actionTimer;

    private Vector2Int location;
    private CreatureCard creatureCard;


    // Initialize is our own function which is called by HandSlot
    public override void Initialize(
        Card card, GameManager.Team team, HandSlot slot)
    {
        base.Initialize(card, team, slot);

        statuses = new List<Status>();
        creatureCard = (CreatureCard) card;

        health = creatureCard.health;
        actionValue = creatureCard.cardBehavior.actionValue;
        actionTime = creatureCard.actionTime;
        healthText.text = health.ToString();
        actionValueText.text = actionValue.ToString();
        actionTimeText.text = actionTime.ToString();

        gridDisplayObject.SetActive(false);
    }

    public override void EnablePreview()
    {
        //set color/transparency
        sprite.color = new Color(0, 0, 0, 0.5f);
        //snap to grid
        gameObject.transform.position = GetCastLocation().transform.position;
    }

    public override void DisablePreview()
    {
        //enable card display
        cardFront.SetActive(true);
        cardBack.SetActive(true);
        //set color/transparency
        sprite.color = new Color(255, 255, 255, 1.0f);
        //disable sprite
        sprite.gameObject.SetActive(false);
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
        //TODO: check if creature already exist in cell
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

    public override HashSet<GameObject> GetCastTargets(GameObject target)
    {
        HashSet<GameObject> targets = new HashSet<GameObject>();
        GridCell cell = target.GetComponent<GridCell>();
        for (int r = 0; r < creatureCard.size.x; r++)
        for (int c = 0; c < creatureCard.size.y; c++)
        {
            Vector2Int rc = new Vector2Int(cell.coordinates.x + r, cell.coordinates.y + c);
            targets.Add(grid.GetCellRC(rc).gameObject);
        }
        return targets;
    }

    public override void Cast(GameObject target)
    {
        GridCell cell = target.GetComponent<GridCell>();
        location = cell.coordinates;

        gameObject.layer = SortingLayer.NameToID("Creatures");

        //TODO: add creature location(s) to CreatureGrid
        foreach (GameObject cellObject in GetCastTargets(target))
        {
            GridCell c = cellObject.GetComponent<GridCell>();
            grid.creatures.Add(c.coordinates, this);
        }

        // Turn CreatureCard into Creature on grid
        gameObject.name = card.cardName;
        // move out of the hierarchy
        transform.SetParent(grid.playerCreaturesParent);
        // move onto grid position
        transform.position = target.transform.position;

        // Enable Grid Creature Display
        gridDisplayObject.SetActive(true);
        RectTransform rt = gridDisplayObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(
            42 * creatureCard.size.x, 42 * creatureCard.size.y);
        gridHealthText.text = health.ToString();
        gridActionValueText.text = actionValue.ToString();
        gridActionTimeText.text = actionTime.ToString();

        //disable card display
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        // enable sprite display
        sprite.gameObject.SetActive(true);
        // set color/transparency to normal
        sprite.color = new Color(255, 255, 255, 1.0f);

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
        Destroy(this.gameObject);
    }

    private IEnumerator DoAction()
    {
        actionTimer = 0;
        while (health > 0)
        {
            DoStatuses();
            actionTimer += Time.deltaTime;
            //creatureSlot.attackSlider.value = actionTimer / actionTime;
            if (actionTimer / actionTime >= 1)
            {
                Debug.Log("Health: " + health);
                //TODO: Get target on grid

                actionTimer = 0;
                //creatureSlot.attackSlider.value = 0;
            }
            yield return null;
        }

        // when health <= 0
        DestroySelf();
    }

    public void DoStatuses()
    {
        Debug.Log("Statuses Count: " + statuses.Count);
        for (int i = 0; i < statuses.Count; i++)
        {
            Status status = statuses[i];
            Debug.Log("Status: " + status.statusType.ToString());
            Debug.Log("Stacks: " + status.stacks.ToString());
            switch (statuses[i].statusType)
            {
                case Card.StatusType.Frozen:
                    actionTimer -= Time.deltaTime;
                    if (gameTimer.elapsedTime - statuses[i].startTime > 1f)
                    {
                        statuses[i] = new Status(status.statusType, status.stacks - 1, gameTimer.elapsedTime);
                    }
                    break;
                case Card.StatusType.Bleeding:
                    if (gameTimer.elapsedTime - statuses[i].startTime > 1f)
                    {
                        Damage(status.stacks);
                        statuses[i] = new Status(status.statusType, status.stacks - 1, gameTimer.elapsedTime);
                    }
                    break;
                default:
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
        SetHealth(health -= hp);
    }

    public void Heal(int hp)
    {
        SetHealth(health += hp);
    }

    public void SetHealth(int hp)
    {
        health = hp;

        // change health display
        gridHealthText.text = health.ToString();
    }

    public int GetHealth()
    {
        return health;
    }

    public void ApplyStatus(Card.StatusType statusType, int stacks)
    {   
        for (int i = 0; i < statuses.Count; i++)
        {
            Status s = statuses[i];
            if (s.statusType == statusType)
            {
                s.stacks += stacks;
                return;
            }
        }
        statuses.Add(new Status(statusType, stacks, gameTimer.elapsedTime));
    }
}
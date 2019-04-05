using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CreatureCardManager : CardManager, IEntity
{

    public List<Card.Status> statuses;

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

        statuses = new List<Card.Status>();
        creatureCard = (CreatureCard) card;

        health = creatureCard.health;
        actionValue = creatureCard.cardBehavior.actionValue;
        actionTime = creatureCard.actionTime;
        healthText.text = health.ToString();
        actionValueText.text = actionValue.ToString();
        actionTimeText.text = actionTime.ToString();

        gridDisplayObject.SetActive(false);
    }

    public override void EnablePreview(GameObject target)
    {
        //disable card display
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        //enable sprite
        sprite.gameObject.SetActive(true);
        //set color/transparency
        sprite.color = new Color(0, 0, 0, 0.5f);
        //snap to grid
        gameObject.transform.position = target.transform.position;
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

    public override void Cast(GameObject target)
    {
        GridCell cell = target.GetComponent<GridCell>();
        location = cell.coordinates;

        gameObject.layer = SortingLayer.NameToID("Creatures");

        //add creature location to CreatureGrid
        for (int r = 0; r < creatureCard.size.x; r++)
        for (int c = 0; c < creatureCard.size.y; c++)
        {
            Vector2Int rc = new Vector2Int(location.x + r, location.y + c);
            grid.creatures.Add(rc, this);
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
        StartCoroutine(ExecuteStatuses());
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

    public IEnumerator ExecuteStatuses()
    {
        // deal with hecking statuses :(
        yield return null;
    }
}
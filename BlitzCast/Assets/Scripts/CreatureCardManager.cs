using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CreatureCardManager : CardManager, IEntity
{
    public TMP_Text healthText;
    public TMP_Text actionValueText;
    public TMP_Text actionTimeText;
    public GameObject sprite;
    public Animator animator;
    public List<Card.Status> statuses;

    private int health;
    private int actionValue;
    private int actionTime;
    private float actionTimer;

    private Vector2Int location;

    public override void Initialize(Card card, GameManager.Team team, HandSlot slot)
    {
        this.card = card;
        this.team = team;
        this.slot = slot;

        statuses = new List<Card.Status>();

        CreatureCard creatureCard = (CreatureCard) card;
        health = creatureCard.health;
        actionValue = creatureCard.cardBehavior.actionValue;
        actionTime = creatureCard.actionTime;
        healthText.text = health.ToString();
        actionValueText.text = actionValue.ToString();
        actionTimeText.text = actionTime.ToString();
        animator.runtimeAnimatorController = creatureCard.animator;
    }

    public override void Cast(GameObject target)
    {
        CreatureCard creatureCard = (CreatureCard) card;
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
        //RectTransform rt = (RectTransform) gameObject.transform;
        //rt.sizeDelta = new Vector2(42f, 42f);

        // move out of the hierarchy
        transform.SetParent(grid.playerCreaturesParent);
        // move onto grid position
        transform.position = target.transform.position;

        //disable card display
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        // enable sprite display
        sprite.SetActive(true);
        //set color/transparency to normal
        sprite.GetComponent<Image>().color = new Color(255, 255, 255, 1.0f);

        // Start action timer coroutine
        StartCoroutine(ExecuteStatuses());
        StartCoroutine(DoAction());
    }

    public override void DestroySelf()
    {
        CreatureCard creatureCard = (CreatureCard) card;
        //delete creature locations from CreatureGrid
        for (int r = 0; r < creatureCard.size.x; r++)
            for (int c = 0; c < creatureCard.size.y; c++)
            {
                Vector2Int rc = new Vector2Int(location.x + r, location.y + c);
                grid.creatures.Remove(rc);
            }
        Destroy(this.gameObject);
    }

    public override GameObject GetCastLocation()
    {
        CreatureCard creatureCard = (CreatureCard) card;

        // Get first GridCell under cursor
        GameObject cellObject = gameManager.GetFirstUnderCursor<GridCell>();
        GridCell cell = cellObject != null ? cellObject.GetComponent<GridCell>() : null;

        if (cell != null && 
            cell.coordinates.x >= grid.size.x / 2 &&
            cell.coordinates.y + creatureCard.size.y - 1 < grid.size.y &&
            cell.coordinates.x + creatureCard.size.x - 1 < grid.size.x)
            //TODO: check if creature already exist in cell
        {
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

    public override void EnablePreview(GameObject target)
    {
        CreatureCard creatureCard = (CreatureCard) card;
        //disable card display
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        //enable sprite
        sprite.SetActive(true);
        //set color/transparency
        sprite.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        //snap to grid
        gameObject.transform.position = target.transform.position;
    }

    public override void DisablePreview()
    {
        //enable card display
        cardFront.SetActive(true);
        cardBack.SetActive(true);
        //set color/transparency
        sprite.GetComponent<Image>().color = new Color(255, 255, 255, 1.0f);
        //disable sprite
        sprite.SetActive(false);
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
                //TODO: Update for Grid system
                //gameManager.GetCreatureAttackTarget(creatureSlot.index, creatureSlot.GetTeam())
                //    .Damage(attack);

                actionTimer = 0;
                //creatureSlot.attackSlider.value = 0;
            }
            yield return null;
        }

        // health <= 0
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
        //creatureSlot.healthText.text = health.ToString();
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
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CreatureCardManager : CardManager
{
    public TMP_Text healthText;
    public TMP_Text actionValueText;
    public TMP_Text actionTimeText;
    public GameObject sprite;
    public Animator animator;

    private int health;
    private int actionValue;
    private int actionTime;
    private float actionTimer;

    public override void Initialize(Card card, GameManager.Team team, HandSlot slot)
    {
        this.card = card;
        this.team = team;
        this.slot = slot;

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
        gameObject.layer = SortingLayer.NameToID("Creatures");
        CreatureCard creatureCard = (CreatureCard) card;

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
        StartCoroutine(DoAction());
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

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public override GameObject GetCastTarget()
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
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreatureCardManager : CardManager
{
    public Text healthText;
    public Text actionValueText;
    public Text actionTimeText;
    public GameObject sprite;
    public Animator animator;

    private int health;
    private int actionValue;
    private int actionTime;
    private float actionTimer;

    public new void Initialize(Card card, GameManager.Team team)
    {
        base.Initialize(card, team);

        CreatureCard creatureCard = (CreatureCard) card;
        health = creatureCard.health;
        actionValue = creatureCard.behavior.actionValue;
        actionTime = creatureCard.actionTime;

        healthText.text = health.ToString();
        actionValueText.text = actionValue.ToString();
        actionTimeText.text = actionTime.ToString();
    }

    public override void Cast(List<GameObject> Targets)
    {

        CreatureCard creatureCard = (CreatureCard)card;

        // Turn CreatureCard into Creature on grid
        gameObject.name = card.cardName;
        RectTransform rt = (RectTransform)gameObject.transform;
        rt.sizeDelta = new Vector2(64f, 64f);

        sprite.SetActive(true);
        animator.runtimeAnimatorController = creatureCard.animator;

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

    public override bool ValidateCast()
    {
        CreatureCard creatureCard = (CreatureCard)card;
        List<GameObject> hitObjects = gameManager.GetAllUnderCursor();
        foreach (GameObject g in hitObjects)
        {
            GridCell cell = g.GetComponent<GridCell>();
            if (cell != null && 
                cell.coordinates.x >= grid.size.x / 2 &&
                cell.coordinates.y + creatureCard.size.y - 1 < grid.size.y &&
                cell.coordinates.x + creatureCard.size.x - 1 < grid.size.x
                )
            {
                return true;
            }
        }
        return false;
    }
    public override void EnablePreview(GridCell cell)
    {
        CreatureCard creatureCard = (CreatureCard)card;
        //disable card display
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        //enable sprite
        sprite.SetActive(true);
        //set color/transparency
        sprite.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        //snap to grid
        gameObject.transform.position = cell.transform.position;
    }

    public override void DisablePreview ()
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
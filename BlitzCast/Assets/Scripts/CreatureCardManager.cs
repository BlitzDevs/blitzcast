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

    private CreatureCard creatureCard;

    void Start()
    {
        creatureCard = (CreatureCard) card;
        health = creatureCard.health;
        actionValue = creatureCard.behavior.actionValue;
        actionTime = creatureCard.actionTime;

        healthText.text = health.ToString();
        actionValueText.text = actionValue.ToString();
        actionTimeText.text = actionTime.ToString();
    }

    public override void Cast(List<GameObject> Targets)
    {

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
}
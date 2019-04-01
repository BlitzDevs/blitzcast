using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreatureCardManager : CardManager
{
    public Text healthText;
    public Text actionValueText;
    public Text actionTimeText;
    public GameObject sprite;
    public Animator animator;
    public CardManager cardManager;

    private int health;
    private int actionValue;
    private int actionTime;
    private float actionTimer;

    void Start()
    {
        CreatureCard creatureCard = (CreatureCard)card;
        health = creatureCard.health;
        actionValue = creatureCard.behavior.actionValue;
        actionTime = creatureCard.actionTime;

        healthText.text = health.ToString();
        actionValueText.text = actionValue.ToString();
        actionTimeText.text = actionTime.ToString();
    }

    public void CardIntoCreature()
    {
        //TODO:
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
        creatureSlot.healthText.text = health.ToString();
    }

    public int GetHealth()
    {
        return health;
    }

    public void DestroySelf()
    {
        Debug.Log(creatureCard.cardName + " died.");
        creatureSlot.statsDisplay.SetActive(false);
        Destroy(this.gameObject);
    }

}
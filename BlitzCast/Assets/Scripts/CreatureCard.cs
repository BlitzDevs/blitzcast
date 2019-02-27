using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card, IEntity
{

    public RuntimeAnimatorController animator;
    [SerializeField] private int health;
    [SerializeField] private int attack;
    [SerializeField] private int speed;

    private CreatureManager creatureManager;


    public override Card Clone()
    {
        CreatureCard copy = (CreatureCard)CreateInstance(typeof(CreatureCard));
        copy.cardName = this.cardName;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.behavior = this.behavior;
        copy.animator = this.animator;

        copy.health = this.health;
        copy.attack = this.attack;
        copy.speed = this.speed;

        return copy;
    }

    public override void Cast(GameObject target)
    {
        CreatureSlot creatureSlot = target.GetComponent<CreatureSlot>();
        if (creatureSlot == null)
        {
            Debug.LogError("Creature card target is not creature slot");
        }

        creatureSlot.SetCard(creatureManager.gameObject);
        creatureManager.CardIntoSprite();
        creatureManager.StartCoroutine(BeginAttacking());
    }

    IEnumerator BeginAttacking()
    {
        while (health > 0)
        {
            yield return new WaitForSecondsRealtime(speed);
            Debug.Log(cardName + " attack placeholder");
        }

        // health <= 0
        creatureManager.DestroySelf();
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
        creatureManager.SetStatsTexts(GetStats());
    }

    public Vector3Int GetStats()
    {
        return new Vector3Int(health, attack, speed);
    }



    public void SetCreatureManager(CreatureManager creatureManager)
    {
        this.creatureManager = creatureManager;
    }

}

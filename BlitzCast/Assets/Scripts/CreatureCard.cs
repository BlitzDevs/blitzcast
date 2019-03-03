using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card
{

    public RuntimeAnimatorController animator;
    public int health;
    public int attack;
    public int speed;

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

    public override void Cast(GameObject selfObject, GameObject target)
    {
        CreatureSlot creatureSlot = target.GetComponent<CreatureSlot>();
        if (creatureSlot == null)
        {
            Debug.LogError("Creature card target is not creature slot");
        }

        creatureSlot.SetObject(creatureManager.gameObject);
        creatureManager.CardIntoCreature(creatureSlot);
    }

    public void SetCreatureManager(CreatureManager creatureManager)
    {
        this.creatureManager = creatureManager;
    }

}

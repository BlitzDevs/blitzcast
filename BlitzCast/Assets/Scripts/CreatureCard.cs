using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card
{

    public RuntimeAnimatorController animator;
    public int health;
    public int actionTime;
    public Vector2Int size;
    public Behavior behavior;



    public override Card Clone()
    {
        CreatureCard copy = (CreatureCard)CreateInstance(typeof(CreatureCard));
        copy.cardName = this.cardName;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.behavior = this.behavior;
        copy.size = this.size;

        copy.animator = this.animator;
        copy.health = this.health;
        copy.actionTime = this.actionTime;

        return copy;
    }

}
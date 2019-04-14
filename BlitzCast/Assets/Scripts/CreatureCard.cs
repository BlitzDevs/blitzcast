using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card
{

    public enum CreatureTarget
    {
        Front,
        Adjacent,
        Row,
        Column,
        AllCreatures,
        All
    }

    public int health = 1;
    public int actionTime = 1;
    public Vector2Int size = Vector2Int.one;
    public List<Entity.StatusModifier> statusModifiers = new List<Entity.StatusModifier>();
    public List<Entity.StatModifier> statModifiers = new List<Entity.StatModifier>();


    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public override Card Clone()
    {
        CreatureCard copy = (CreatureCard) base.Clone();
        copy.health = health;
        copy.actionTime = actionTime;
        copy.size = size;
        copy.statusModifiers = statusModifiers;
        copy.statModifiers = statModifiers;
        return copy;
    }

}
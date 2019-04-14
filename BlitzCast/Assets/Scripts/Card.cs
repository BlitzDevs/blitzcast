using System;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    // A ScriptableObject is a Unity object whose properties we can edit inside
    // the editor.
    // This class should contain only the properties unique to a card.
    // CardManager deals with card actions/status/etc while in game.

    public enum Race
    {
        Generic,
        Dwarf,
        Elf,
        Auto
    }

    public enum Action
    {
        Damage,
        Heal,
        IncreaseHP,
        Destroy,
        Counter
    }

    public enum TargetArea
    {
        Single,
        SingleCreature,
        Cross,
        Square,
        Row,
        Column,
        AllCreatures,
        All
    }

    [Serializable]
    public struct Behavior
    {
        public Action action;
        public int actionValue;
        public TargetArea targetArea;
        public Entity.Status.StatusType statusInflicted;
        public int stacks;
    }

    public string cardName = "New Card";
    public string description = "Something cool?";
    public int spriteAnimateSpeed = 30;
    public Color color = Color.white;
    public int castTime = 1;
    public int redrawTime = 1;
    public Race race = Race.Generic;
    public Behavior cardBehavior;
    public float actionChance = 1.0f; // should be float btwn 0.0f & 1.0f


    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public virtual Card Clone()
    {
        Card copy = (CreatureCard)CreateInstance(typeof(CreatureCard));
        copy.cardName = cardName;
        copy.description = description;
        copy.castTime = castTime;
        copy.redrawTime = redrawTime;
        copy.cardBehavior = cardBehavior;
        copy.color = color;
        copy.actionChance = actionChance;
        return copy;
    }
}
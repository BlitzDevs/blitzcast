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
        Automaton,
        Undead,
        Critter
    }

    public enum Action
    {
        None,
        Damage,
        Heal,
        Destroy,
        Counter
    }

    public enum StatChange
    {
        None,
        SetHealth,
        IncreaseHealth,
        IncreaseSpeed
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
        [Range(0, 30)] public int actionValue;
        public StatChange statChange;
        [Range(-100, 100)] public int statChangeValue;
        public TargetArea targetArea;
        public Entity.Status.StatusType statusInflicted;
        [Range(0, 10)] public int stacks;
    }

    public string cardName = "New Card";
    public string description = "Something cool?";
    [Range(0, 100)] public int spriteAnimateSpeed = 30;
    public Color color = Color.white;
    [Range(0, 30)] public int castTime = 1;
    [Range(0, 30)] public int redrawTime = 1;
    public Race race = Race.Generic;
    public Behavior cardBehavior;
    [Range(0, 100)] public int actionChance = 100;


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
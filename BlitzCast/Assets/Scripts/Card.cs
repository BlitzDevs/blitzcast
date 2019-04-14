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

    // Filter targets based on condition
    public enum Condition
    {
        None,
        HPGreaterThan,
        HPLessThan,
        Race,
        Status,
        Friendly,
        Enemy
    }


    public string cardName = "New Card";
    public string description = "Something cool?";
    [Range(0, 100)] public int spriteAnimateSpeed = 30;
    public Color color = Color.white;
    [Range(0, 30)] public int castTime = 1;
    [Range(0, 30)] public int redrawTime = 1;
    public Race race = Race.Generic;
    public Action action;
    [Range(0, 30)] public int actionValue;
    public StatChange statChange;
    [Range(-100, 100)] public int statChangeValue;
    public Entity.Status.StatusType statusInflicted;
    [Range(0, 10)] public int stacks;
    [Range(0, 100)] public int actionChance = 100;
    public Condition condition;
    public int conditionValue;


    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public virtual Card Clone()
    {
        Card copy = (CreatureCard)CreateInstance(typeof(CreatureCard));
        cardName = copy.cardName;
        description = copy.description;
        spriteAnimateSpeed = copy.spriteAnimateSpeed;
        color = copy.color;
        castTime = copy.castTime;
        redrawTime = copy.redrawTime;
        race = copy.race;
        action = copy.action;
        actionValue = copy.actionValue;
        statChange = copy.statChange;
        statChangeValue = copy.statChangeValue;
        statusInflicted = copy.statusInflicted;
        stacks = copy.stacks;
        actionChance = copy.actionChance;
        copy.condition = condition;
        copy.conditionValue = conditionValue;
        return copy;
    }
}
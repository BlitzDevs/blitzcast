using System;
using UnityEngine;

public abstract class Card : ScriptableObject
{
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

    public enum Status
    {
        None,
        Confusion,
        Wounded,
        Frozen,
        Poisoned,
        Rally
    }

    [Serializable]
    public struct Behavior
    {
        public Action action;
        public int actionValue;
        public Condition condition;
        public int conditionValue;
        public TargetArea targetArea;
        public Status statusInflicted;
        public int statusValue;
    }

    public Race race;
    public string cardName;
    public string description;
    public Sprite art;
    public Behavior cardBehavior;
    public int castTime;
    public int redrawTime;

    public abstract Card Clone();
}
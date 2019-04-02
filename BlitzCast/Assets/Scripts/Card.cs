using System;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    public enum Action
    {
        Damage,
        Heal,
        Destroy
    }

    public enum TargetArea
    {
        Single,
        Cross,
        Square,
        Row,
        Column,
        All
    }

    public enum Condition
    {
        None,
        HPGreaterThan,
        HPLessThan,
        Race,
        Status,
        Friendly
    }

    public enum Status
    {
        None,
        Confusion,
        Wounded,
        Frozen
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
    }

    public string cardName;
    public string description;
    public Sprite art;
    public Behavior cardBehavior;
    public int castTime;
    public int redrawTime;
    public abstract Card Clone();
}
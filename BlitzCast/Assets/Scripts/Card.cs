using System;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    public enum Action
    {
        Damage,
        Heal,
        Destroy,
        GiveStatus
    }

    public enum ActionShape
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
        HPGreaterThan,
        HPLessThan,
        Race,
        Status,
        Friendly
    }

    [Serializable]
    public struct Behavior
    {
        public Action action;
        public int actionValue;
        public Condition condition;
        public int conditionValue;
        public GameManager.Team targetTeam;
        public ActionShape actionShape;
    }

    public string cardName;
    public string description;
    public Sprite art;
    public int castTime;
    public int redrawTime;
    public abstract Card Clone();
}
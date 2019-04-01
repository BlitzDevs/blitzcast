using System;
using System.Collections;
using System.Collections.Generic;
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
        Action action;
        int actionValue;
        Condition condition;
        int conditionValue;
        GameManager.Team targetTeam;
        ActionShape actionShape;
    }

    public string cardName;
    public string description;
    public Sprite art;
    public int castTime;
    public int redrawTime;
    public abstract Card Clone();
}
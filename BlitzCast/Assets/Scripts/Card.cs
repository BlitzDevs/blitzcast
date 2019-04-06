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

    public enum StatusType
    {
        None,
        Confused,
        Wounded,
        Frozen,
        Bleeding
    }

    [Serializable]
    public struct Behavior
    {
        public Action action;
        public int actionValue;
        public TargetArea targetArea;
        public StatusType statusInflicted;
        public int stacks;
    }


    public Race race;
    public string cardName;
    public string description;
    public int castTime;
    public int redrawTime;
    public Behavior cardBehavior;
    public Sprite art;
    public RuntimeAnimatorController animator;

    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public abstract Card Clone();
}
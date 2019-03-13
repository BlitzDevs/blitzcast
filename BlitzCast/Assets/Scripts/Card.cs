using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{

    public enum Status
    {
        Deck,
        Held,
        Dragging,
        Recharging,
        Casting,
        Active
    }

    public enum Action
    {
        Creature,
        DamageTarget,
        DamageAll,
        BuffAttack,
        Counter
    }

    public enum Target
    {
        CastZone,
        HeldCardSlot,
        CreatureSlot,
        Creature,
        CastingCard,
        Opponent,
        User
    }

    [Serializable]
    public struct Behavior
    {
        public Action action;
        public List<Target> targets;
        public int value;
    }

    public string cardName;
    public string description;
    public Sprite art;
    public int castTime;
    public int redrawTime;
    public List<Behavior> behaviors;

    private Status status;

    public abstract Card Clone();
    public abstract void Cast(GameObject selfObject, GameObject target);


    public void SetStatus(Status status)
    {
        this.status = status;
    }

    public bool StatusIs(Status status)
    {
        return this.status == status;
    }

}

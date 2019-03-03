using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{

    public enum CardStatus
    {
        Deck,
        Held,
        Dragging,
        Recharging,
        Casting,
        Active
    }

    public enum CardAction
    {
        Creature,
        DamageTarget,
        DamageAll,
        BuffAttack,
        Counter
    }

    [Serializable]
    public struct CardBehavior
    {
        public CardAction action;
        public int value;
    }

    public string cardName;
    public string description;
    public Sprite art;
    public int castTime;
    public int redrawTime;
    public CardBehavior behavior;

    private CardStatus status;
    private GameManager.Team team;

    public abstract Card Clone();
    public abstract void Cast(GameObject selfObject, GameObject target);


    public void SetTeam(GameManager.Team team)
    {
        this.team = team;
    }

    public void SetStatus(CardStatus status)
    {
        this.status = status;
    }

    public bool StatusIs(CardStatus status)
    {
        return this.status == status;
    }

}

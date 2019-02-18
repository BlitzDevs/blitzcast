using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{

    public enum CardStatus
    {
        Deck,
        Held,
        Recharging,
        Casting,
        Active
    }

    public enum CardBehavior
    {
        Creature,
        DamageTarget,
        DamageAll,
        BuffAttack,
        Counter
    }

    public enum Team
    {
        A,
        B,
        Neutral
    }

    public new string name;
    public string description;
    public Sprite art;
    public int castTime;
    public int redrawTime;
    public List<CardBehavior> behaviors;
    public List<int> behaviorValues;

    public CardStatus status;
    public Team team;

    public abstract Card Clone();

    public Player GetOwner()
    {
        switch (team)
        {
            case Team.A:
                return FindObjectOfType<GameManager>().playerA;
            case Team.B:
                return FindObjectOfType<GameManager>().playerB;
            case Team.Neutral:
                return null;
            default:
                Debug.LogError("Team is null");
                return null;
        }
    }

}

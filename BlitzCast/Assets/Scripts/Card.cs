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
        DamageTarget,
        DamageAll,
        BuffAttack,
        Counter
    }

    public new string name;
    public string description;
    public Sprite art;
    public int timeCost;
    public CardStatus status;
    public List<CardBehavior> behaviors;
    public List<int> behaviorValues;
    
    public abstract Card Clone();

}

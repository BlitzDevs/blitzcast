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

    public new string name;
    public string description;
    public Sprite art;
    public int timeCost;
    public CardStatus status;
    public Castable castable;


    //public abstract void Cast();
    public abstract Card Clone();

}

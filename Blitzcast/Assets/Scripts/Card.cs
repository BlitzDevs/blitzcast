using UnityEngine;

public abstract class Card : ScriptableObject
{

    public new string name;
    public string description;

    public Sprite art;

    public int timeCost;
    

    public abstract void Cast();
}

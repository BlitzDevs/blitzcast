using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Player")]
public class Player : ScriptableObject
{
    [Serializable]
    public struct CardSkin
    {
        public Sprite creatureCard;
        public Sprite spellCard;
        public Sprite handSlot;
    }


    public string username;
    public Caster caster;
    public List<Card> deck;
    public CardSkin skin;
}

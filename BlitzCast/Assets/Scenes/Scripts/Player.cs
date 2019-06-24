using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player contains information about the player which is needed to initiate
/// a game.
/// </summary>
[CreateAssetMenu(fileName = "New Player Data", menuName = "Player")]
public class Player : ScriptableObject
{
    /// <summary>
    /// Holds the sprites to define a card skin.
    /// </summary>
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

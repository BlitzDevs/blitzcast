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

    public string username;
    public Caster caster;
    public List<Card> deck;
    public Color color;
}

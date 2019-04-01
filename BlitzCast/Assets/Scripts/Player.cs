using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Player")]
public class Player : ScriptableObject
{
    public string username;
    public Sprite icon;
    public List<Card> deck;
}

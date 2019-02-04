using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card {

    public override void Cast()
    {
        Debug.Log("Casting a spell..");
    }
}

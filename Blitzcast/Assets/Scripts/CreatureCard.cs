using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card {

    public override void Cast()
    {
        Debug.Log("Casting a creature..");
    }
}

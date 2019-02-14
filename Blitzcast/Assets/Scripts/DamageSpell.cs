using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Spell", menuName = "Damage Spell")]
public class DamageSpell : Castable
{
    public int value;

    public override void Cast(CreatureCard target)
    {
        target.health -= value;
    }

    public override void Cast(SpellCard target)
    {
        Debug.Log("Can't target a spell card");
    }

    public override void Cast(Player target)
    {
        target.AddHealth(-value);
    }
}

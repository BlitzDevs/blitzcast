using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card
{

    public override Card Clone()
    {
        SpellCard copy = (SpellCard) CreateInstance(typeof(SpellCard));

        copy.cardName = this.cardName;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.behavior = this.behavior;

        return copy;
    }

}

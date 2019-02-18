using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card {

    public override Card Clone()
    {
        SpellCard copy = (SpellCard) CreateInstance(typeof(SpellCard));
        copy.name = this.name;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.status = this.status;
        copy.behaviors = this.behaviors;
        copy.behaviorValues = this.behaviorValues;
        return copy;
    }
}

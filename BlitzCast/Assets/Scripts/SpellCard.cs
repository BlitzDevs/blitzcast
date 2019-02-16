using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card {

    public override Card Clone()
    {
        SpellCard copy = new SpellCard
        {
            name = this.name,
            description = this.description,
            art = this.art,
            timeCost = this.timeCost,
            status = this.status,
            behaviors = this.behaviors,
            behaviorValues = this.behaviorValues
        };
        return copy;
    }
}

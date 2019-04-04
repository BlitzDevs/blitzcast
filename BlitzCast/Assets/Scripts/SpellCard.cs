using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card
{

    // Conditions mean cast spell only if target is/has x
    public enum Condition
    {
        None,
        HPGreaterThan,
        HPLessThan,
        Race,
        Status,
        Friendly,
        Enemy
    }

    public Condition condition;
    public int conditionValue;


    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public override Card Clone()
    {
        SpellCard copy = (SpellCard) CreateInstance(typeof(SpellCard));

        copy.cardName = this.cardName;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.cardBehavior = this.cardBehavior;

        copy.condition = this.condition;
        copy.conditionValue = this.conditionValue;

        return copy;
    }

}

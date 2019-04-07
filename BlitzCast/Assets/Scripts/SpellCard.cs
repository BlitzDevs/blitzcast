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
        SpellCard copy = (SpellCard) base.Clone();
        copy.condition = condition;
        copy.conditionValue = conditionValue;
        return copy;
    }
}

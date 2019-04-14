using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card
{

    public enum SpellTarget
    {
        Single,
        SingleCreature,
        Cross,
        Square,
        Row,
        Column,
        AllCreatures,
        All
    }

    public SpellTarget targetArea;


    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public override Card Clone()
    {
        SpellCard copy = (SpellCard) base.Clone();
        return copy;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card {

    /*
    public enum SpellType {
        Damage,
        Counter,
        Enchant
    }

    public SpellType spellType;
    public int spellValue;

    public override void Cast()
    {
        status = CardStatus.Casting;

        Debug.Log("Casting a spell..");
        switch (spellType)
        {
            case SpellType.Damage:
                {
                    Debug.Log("Deal damage");
                    break;
                }
            case SpellType.Counter:
                {
                    Debug.Log("Counter a spell");
                    break;
                }
            case SpellType.Enchant:
                {
                    Debug.Log("Create an effect");
                    break;
                }
        }
    }
    */

    public override Card Clone()
    {
        SpellCard copy = new SpellCard();
        copy.name = this.name;
        copy.description = this.description;
        copy.art = this.art;
        copy.timeCost = this.timeCost;
        copy.status = this.status;
        copy.castable = this.castable;
        return copy;
    }
}

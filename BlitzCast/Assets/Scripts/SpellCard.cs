using UnityEngine;

// Mark a ScriptableObject-derived type to be automatically listed in the
// Assets/Create submenu, so that instances of the type can be easily created
// and stored in the project as ".asset" files.
[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]

/// <summary>
/// Contains all the properties of a Card and properties unique to a Spell.
/// </summary>
/// <seealso cref="Card"/>
/// <seealso cref="SpellCard"/>
public class SpellCard : Card
{

    /// <summary>
    /// Types of action targets for Spell, in relation to the cast location
    /// (which is determined by mouse position).
    /// </summary>
    public enum SpellTarget
    {
        [Display("SINGLE", "1", 0, 255, 110)] Single,
        [Display("SINGLE", "1CR", 0, 255, 110)] SingleCreature,
        [Display("CROSS", "CRO", 0, 255, 110)] Cross,
        [Display("SQUARE", "SQR", 0, 255, 110)] Square,
        [Display("ROW", "ROW", 0, 255, 40)] Row,
        [Display("COLUMN", "COL", 160, 0, 255)] Column,
        [Display("CREATURES", "CRE", 255, 110, 0)] AllCreatures,
        [Display("ALL", "ALL", 220, 0, 0)] All
    }

    public SpellTarget targetArea;


    /// <summary>
    /// Create a copy of this card.
    /// Cannot use newCard = oldCard because it becomes a reference.
    /// For SpellCard.Clone(), SpellCard's additional properties are also
    /// copied and returned in addition to the Card properties.
    /// </summary>
    public override Card Clone()
    {
        SpellCard copy = (SpellCard) base.Clone();
        copy.targetArea = targetArea;
        return copy;
    }
}

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains all the properties of a Card and properties unique to a Creature.
/// </summary>
/// <seealso cref="Card"/>
/// <seealso cref="CreatureCard"/>
[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card
{
    // FOR REFERENCE:
    // [CreateAssetMenu]    An Attribute from UnityEngine; Allow a
    //                      ScriptableObject derived object to be listed in the
    //                      Assets/Create submenu, so that instances of the type
    //                      can be easily created and stored in the project as
    //                      ".asset" files.

    /// <summary>
    /// Types of action targets for Creature, in relation to the creature's
    /// coordinate position on the CreatureGrid.
    /// </summary>
    public enum CreatureTarget
    {
        [Display("FRONT", "FRO", 0, 255, 110)]
        Front,
        [Display("ADJACENT", "ADJ", 255, 240, 0)]
        Adjacent,
        [Display("ROW", "ROW", 0, 255, 40)]
        Row,
        [Display("COLUMN", "COL", 160, 0, 255)]
        Column,
        [Display("CREATURES", "CRE", 255, 110, 0)]
        AllCreatures,
        [Display("ALL", "ALL", 220, 0, 0)]
        All
    }

    // properties unique to a Creature
    public int health = 1;
    public int actionTime = 1;
    public Vector2Int size = Vector2Int.one;
    public CreatureTarget targetArea;
    // optional StatusModifier or StatModifier (see Entity)
    // which a Creature can begin with
    public List<Entity.StatusModifier> statusModifiers = new List<Entity.StatusModifier>();
    public List<Entity.StatModifier> statModifiers = new List<Entity.StatModifier>();


    /// <summary>
    /// Create a copy of this card.
    /// Cannot use newCard = oldCard because it becomes a reference.
    /// For CreatureCard.Clone(), CreatureCard's additional properties are also
    /// copied and returned in addition to the Card properties.
    /// </summary>
    public override Card Clone()
    {
        CreatureCard copy = (CreatureCard) base.Clone();
        copy.health = health;
        copy.actionTime = actionTime;
        copy.size = size;
        copy.statusModifiers = statusModifiers;
        copy.statModifiers = statModifiers;
        return copy;
    }

}
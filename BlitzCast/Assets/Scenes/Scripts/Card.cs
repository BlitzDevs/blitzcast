using UnityEngine;

/// <summary>
/// The abstract Card that other Cards should inherit from.
/// Contains all the properties unique to a generic card.
/// </summary>
/// <seealso cref="CreatureCard"/>
/// <seealso cref="SpellCard"/>
public abstract class Card : ScriptableObject
{
    // FOR REFERENCE:
    // A ScriptableObject is a Unity object whose properties we can edit inside
    // the editor.

    // The [Range(min, max)] fields allow slider inside the Unity editor
    // and also limits the values.

    // This class should contain only the properties unique to a card.
    // CardManager deals with card actions/status/etc while in game.


    /// <summary>
    /// The Race of a Card.
    /// </summary>
    public enum Race
    {
        [Display("Generic", "GEN", 255, 255, 255)] Generic,
        [Display("Automaton", "AUTO", 100, 150, 200)] Automaton,
        [Display("Undead", "UND", 120, 0, 190)] Undead,
        [Display("Critter", "CRI", 40, 220, 40)] Critter
    }

    /// <summary>
    /// Contains all the properties unique to a card.
    /// </summary>
    /// <note>
    /// Shared between Creature and Spell.
    /// Creature should not be able to counter, however.
    /// </note>
    public enum Action
    {
        [Display("NONE", "-", 255, 255, 255)] None,
        [Display("DAMAGE", "DMG", 255, 255, 255)] Damage,
        [Display("HEAL", "HEAL", 255, 255, 255)] Heal,
        [Display("DESTROY", "DST", 255, 255, 255)] Destroy,
        [Display("COUNTER", "CTR", 255, 255, 255)] Counter
    }

    /// <summary>
    /// Possible status types that can be applied on an Entity.
    /// </summary>
    public enum Status
    {
        [Display("NONE", "-", 255, 255, 255)] None,
        [Display("CLUMSY", "CLM", 40, 255, 0)] Clumsy,
        [Display("WOUND", "WND", 220, 0, 0)] Wound,
        [Display("STUN", "STN", 255, 220, 10)] Stun,
        [Display("POISON", "PSN", 140, 10, 255)] Poison,
        [Display("SHIELD", "SHD", 20, 70, 255)] Shield
    }

    /// <summary>
    /// Stat change type to be applied on a target Entity.
    /// </summary>
    public enum StatChange
    {
        [Display("NONE", "-", 255, 255, 255)] None,
        [Display("SET HEALTH", "SET", 0, 200, 255)] SetHealth,
        [Display("HEALTH", "HP", 255, 0, 0)] Health,
        [Display("SPEED", "SPD", 255, 255, 0)] Speed
    }

    /// <summary>
    /// Condition which can be used to filter a list of targets.
    /// </summary>
    public enum Condition
    {
        [Display("NONE", "-", 255, 255, 255)] None,
        [Display("HP>", "HP>", 220, 0, 0)] HPGreaterThan,
        [Display("HP<", "HP<", 0, 0, 220)] HPLessThan,
        [Display("RACE", "RAC", 100, 100, 100)] Race,
        [Display("STATUS", "STS", 100, 100, 100)] Status,
        [Display("FRIENDLY", "FRI", 0, 220, 0)] Friendly,
        [Display("ENEMY", "ENM", 220, 0, 0)] Enemy,
    }


    // Card variables
    public string cardName = "New Card";
    public string description = "Something cool?";
    public Race race = Race.Generic;
    [Range(0, 30)] public int castTime = 1;
    [Range(0, 30)] public int redrawTime = 1;

    // Display variables
    // spriteAnimateSpeed is used by SpriteSheetAnimator; frames per second
    // the sprite sheet is determined by the card name
    [Range(0, 100)] public int spriteAnimateSpeed = 30;
    public Color color = Color.white;

    // Behavior variables
    // Any of these fields can be set to none/neutral
    public Action action;
    [Range(0, 30)] public int actionValue;
    public StatChange statChange;
    [Range(-100, 100)] public int statChangeValue;
    public Status statusInflicted;
    [Range(0, 10)] public int stacks;
    [Range(0, 100)] public int actionChance = 100;
    public Condition condition;
    public int conditionValue;


    /// <summary>
    /// Create a copy of this card.
    /// Cannot use newCard = oldCard because it becomes a reference.
    /// </summary>
    public virtual Card Clone()
    {
        // copy literally every property of this class
        Card copy = (CreatureCard) CreateInstance(typeof(CreatureCard));
        copy.cardName = cardName;
        copy.description = description;
        copy.spriteAnimateSpeed = spriteAnimateSpeed;
        copy.color = color;
        copy.castTime = castTime;
        copy.redrawTime = redrawTime;
        copy.race = race;
        copy.action = action;
        copy.actionValue = actionValue;
        copy.statChange = statChange;
        copy.statChangeValue = statChangeValue;
        copy.statusInflicted = statusInflicted;
        copy.stacks = stacks;
        copy.actionChance = actionChance;
        copy.condition = condition;
        copy.conditionValue = conditionValue;
        return copy;
    }
}
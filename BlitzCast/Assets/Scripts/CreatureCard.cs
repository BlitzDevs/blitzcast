using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card
{

    public int health;
    public int actionTime;
    public Vector2Int size;


    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public override Card Clone()
    {
        CreatureCard copy = (CreatureCard)CreateInstance(typeof(CreatureCard));
        copy.cardName = this.cardName;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.cardBehavior = this.cardBehavior;
        copy.size = this.size;

        copy.animator = this.animator;
        copy.health = this.health;
        copy.actionTime = this.actionTime;

        return copy;
    }

}
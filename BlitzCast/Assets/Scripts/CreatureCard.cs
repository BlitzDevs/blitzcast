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
        CreatureCard copy = (CreatureCard) base.Clone();
        copy.health = health;
        copy.actionTime = actionTime;
        copy.size = size;
        return copy;
    }

}
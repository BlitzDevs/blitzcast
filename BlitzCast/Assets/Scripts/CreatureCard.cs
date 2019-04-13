using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card
{
    public int health;
    public int actionTime;
    public Vector2Int size;
    public List<Entity.Status> statuses;


    // Cannot use newCard = oldCard because it becomes a reference! Use Clone()!
    public override Card Clone()
    {
        CreatureCard copy = (CreatureCard) base.Clone();
        copy.health = health;
        copy.actionTime = actionTime;
        copy.size = size;
        copy.statuses = statuses;
        return copy;
    }

}
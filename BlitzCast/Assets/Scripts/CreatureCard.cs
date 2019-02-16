using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card {

    public int damage;
    public int speed;
    public int health;
    //public Vector2Int size;
    //private Vector2Int place;

    public override Card Clone()
    {
        CreatureCard copy = new CreatureCard
        {
            name = this.name,
            art = this.art,
            description = this.description,
            timeCost = this.timeCost,
            status = this.status,
            behaviors = this.behaviors,
            behaviorValues = this.behaviorValues,

            damage = this.damage,
            speed = this.speed,
            health = this.health
        };

        return copy;
    }
}

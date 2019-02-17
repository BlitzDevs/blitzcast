using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card, IEntity {

    private int damage;
    private int speed;
    private int health;
    //public Vector2Int size;
    //private Vector2Int place;

    public void Damage(int hp)
    {
        SetHealth(health -= hp);
    }

    public void Heal(int hp)
    {
        SetHealth(health += hp);
    }

    public void SetHealth(int hp)
    {
        health = hp;
        // change health display
        //healthText.text = health.ToString();
    }

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

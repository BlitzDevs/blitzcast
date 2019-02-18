using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Creature Card")]
public class CreatureCard : Card, IEntity {

    [SerializeField] private int health;
    [SerializeField] private int attack;
    [SerializeField] private int speed;

    // possible properties for being on field?
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

    public Vector3Int GetStats()
    {
        return new Vector3Int(health, attack, speed);
    }


    public override Card Clone()
    {

        CreatureCard copy = (CreatureCard) CreateInstance(typeof(CreatureCard));
        copy.name = this.name;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.status = this.status;
        copy.behaviors = this.behaviors;
        copy.behaviorValues = this.behaviorValues;

        copy.health = this.health;
        copy.attack = this.attack;
        copy.speed = this.speed;
        
        return copy;
    }

}

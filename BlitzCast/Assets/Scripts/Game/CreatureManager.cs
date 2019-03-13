using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour, IEntity
{
    private int health;
    private int attack;
    private int speed;

    public Text healthText;
    public Text attackText;
    public Text speedText;
    public GameObject sprite;
    public Animator animator;
    public CardManager cardManager;

    private CreatureSlot creatureSlot;
    private CreatureCard creatureCard;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        creatureCard = (CreatureCard) cardManager.GetCard();
        creatureCard.SetCreatureManager(this);

        health = creatureCard.health;
        attack = creatureCard.attack;
        speed = creatureCard.speed;

        healthText.text = creatureCard.health.ToString();
        attackText.text = creatureCard.attack.ToString();
        speedText.text = creatureCard.speed.ToString();
    }

    public void CardIntoCreature(CreatureSlot creatureSlot)
    {
        gameObject.name = "Creature";
        RectTransform rt = (RectTransform)gameObject.transform;
        rt.sizeDelta = new Vector2(64f, 64f);

        this.creatureSlot = creatureSlot;
        sprite.SetActive(true);
        animator.runtimeAnimatorController = creatureCard.animator;
        cardManager.enabled = false;

        creatureSlot.statsDisplay.SetActive(true);
        creatureSlot.healthText.text = health.ToString();
        creatureSlot.attackText.text = attack.ToString();
        creatureSlot.speedText.text = speed.ToString();

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        float timer = 0;
        while (health > 0)
        {
            timer += Time.deltaTime;
            creatureSlot.attackSlider.value = timer / speed;
            if (timer / speed >= 1)
            {
                gameManager.GetCreatureAttackTarget(creatureSlot.index, creatureSlot.GetTeam())
                    .Damage(attack);
                
                timer = 0;
                creatureSlot.attackSlider.value = 0;
            }
            yield return null;
        }

        // health <= 0
        DestroySelf();
    }

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
        creatureSlot.healthText.text = health.ToString();
    }

    public void DestroySelf()
    {
        Debug.Log(creatureCard.cardName + " died.");
        creatureSlot.statsDisplay.SetActive(false);
        Destroy(this.gameObject);
    }

}

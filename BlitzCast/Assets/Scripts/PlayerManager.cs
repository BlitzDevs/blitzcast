using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Player player;

    public int health;
    public List<string> enchantments;

    public Image iconImage;
    public Text usernameText;
    public Text healthText;
    public Slider healthSlider;
    public GameObject cardSlotsParent;
    public GameObject creatureSlotsParent;

    private GameManager.Team team;
    [SerializeField] private List<Card> playingDeck; // deck in play

    private List<HandSlot> cardSlots;

    private GameManager gameManager;
    private System.Random randomGenerator = new System.Random();
    private int maxHealth;


    public void Initialize(GameManager.Team team, int handSize, int maxHealth)
    {
        gameManager = FindObjectOfType<GameManager>();
        this.team = team;

        // initialize Player
        this.maxHealth = maxHealth;
        SetHealth(maxHealth);

        iconImage.sprite = player.icon;
        usernameText.text = player.username;
        
        // draw first cards
        playingDeck = Clone(player.deck);
        Shuffle();
    }

    public List<Card> Clone(List<Card> original)
    {
        List<Card> newList = new List<Card>(original.Count);

        original.ForEach((item) =>
        {
            newList.Add(item);
        });

        return newList;
    }

    public void Shuffle()
    {
        for (int i = 0; i < playingDeck.Count; i++)
        {
            int j = randomGenerator.Next(0, playingDeck.Count);
            var temp = playingDeck[i];
            playingDeck[i] = playingDeck[j];
            playingDeck[j] = temp;
        }
    }

    public void SetHealthText(int hp)
    {
        healthText.text = hp.ToString();
        healthSlider.value = (float)hp / maxHealth;
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
        SetHealthText(health);
    }

    public int GetHealth()
    {
        return health;
    }

}

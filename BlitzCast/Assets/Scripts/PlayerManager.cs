using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerManager : MonoBehaviour, IEntity
{
    public Player player;

    public int health;
    public List<string> enchantments;

    public Image iconImage;
    public TMP_Text usernameText;
    public TMP_Text healthText;
    public Slider healthSlider;
    public GameObject spellCardPrefab;
    public GameObject creatureCardPrefab;

    public GameManager.Team team;
    [SerializeField] private List<Card> playingDeck; // deck in play

    private List<HandSlot> cardSlots;

    private GameManager gameManager;
    private System.Random randomGenerator = new System.Random();
    private int maxHealth;


    public void Initialize(GameManager.Team team, int maxHealth)
    {
        gameManager = FindObjectOfType<GameManager>();
        this.team = team;

        // initialize Player
        this.maxHealth = maxHealth;
        SetHealth(maxHealth);
        iconImage.sprite = player.icon;
        usernameText.text = player.username;

        // draw first cards
        CloneDeck();
        Shuffle();

        StartCoroutine(ExecuteStatuses());
    }

    public Card DrawTop()
    {
        Card temp = playingDeck[0];
        playingDeck.RemoveAt(0);

        if (playingDeck.Count == 0)
        {
            CloneDeck();
            Shuffle();
        }

        return temp;
    }

    public void CloneDeck()
    {
        playingDeck = new List<Card>(player.deck.Count);

        foreach (Card c in player.deck)
        {
            playingDeck.Add(c);
        }
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

    public IEnumerator ExecuteStatuses()
    {
        // deal with statuses later
        yield return null;
    }
}

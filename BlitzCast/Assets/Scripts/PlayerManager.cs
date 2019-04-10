using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class PlayerManager : MonoBehaviour, IEntity
{
    public Player player;
    public GameManager.Team team;
    public int health;
    public List<Status> statuses;

    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthSlider;
    public GameObject spellCardPrefab;
    public GameObject creatureCardPrefab;

    [SerializeField] private List<Card> playingDeck; // deck in play

    private GameManager gameManager;
    private System.Random randomGenerator = new System.Random();
    private int maxHealth;


    public void Initialize(GameManager.Team team, int maxHealth)
    {
        gameManager = FindObjectOfType<GameManager>();
        statuses = new List<Status>();
        this.team = team;

        // initialize Player
        this.maxHealth = maxHealth;
        SetHealth(maxHealth);
        iconImage.sprite = player.icon;
        usernameText.text = player.username;

        // draw first cards
        CloneDeck();
        Shuffle();
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
        SetHealth(Math.Min(health + hp, maxHealth));
    }

    public void IncreaseHP(int hp)
    {
        maxHealth += hp;
        SetHealth(health + hp);
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

    void Update()
    {
        DoStatuses();
    }

    void DoStatuses()
    {

    }

    void IEntity.DoStatuses()
    {
        throw new System.NotImplementedException();
    }

    public void ApplyStatus(Card.StatusType statusType, int stacks)
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            Status s = statuses[i];
            if (s.statusType == statusType)
            {
                s.stacks += stacks;
                return;
            }
        }

        statuses.Add(new Status(statusType, stacks, gameManager.timer.elapsedTime));
    }

    public List<Status> GetStatuses()
    {
        return statuses;
    }
}
    

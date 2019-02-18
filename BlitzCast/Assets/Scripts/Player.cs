using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IEntity
{

    public string username;
    public GameObject handArea;
    public List<Card> deck; // original deck
    public List<string> enchantments;
    public int handSize = 4;

    public Text usernameText;
    public Text healthText;

    private Card.Team team;
    private int health;
    private int attack;
    private int speed;
    [SerializeField] private List<Card> playingDeck; // deck in play
    [SerializeField] private List<Card> hand;
    [SerializeField] private List<CardSlot> cardSlots;

    private GameManager gm;
    private System.Random randomGenerator = new System.Random();

    public void Initialize(Card.Team team)
    {
        gm = FindObjectOfType<GameManager>();

        // initialize Player
        SetHealth(100);
        attack = 0;
        speed = 0;
        this.team = team;

        usernameText.text = username;
        hand = new List<Card>(handSize);

        // initialize CardSlots
        if (cardSlots.Count != handSize)
        {
            Debug.LogError("Player card slots not match hand size");
        }
        for (int i = 0; i < cardSlots.Count; i++)
        {
            cardSlots[i].index = i + 1; // +1 so index is natural counting nums
        }

        // draw first cards
        playingDeck = CardUtilities.Clone(deck);
        CardUtilities.Shuffle(playingDeck, randomGenerator);
        Draw(handSize);
    }

    public void Draw()
    {
        if (playingDeck.Count == 0) // Reshuffle if empty
        {
            playingDeck = CardUtilities.Clone(deck);
            CardUtilities.Shuffle(playingDeck, randomGenerator);
        }

        // Copy card from playingDeck, take out of playingDeck
        Card newCard = playingDeck[0].Clone();
        playingDeck.RemoveAt(0);
        newCard.status = Card.CardStatus.Held;
        newCard.team = this.team;

        int drawIndex;
        if (hand.Count < handSize)
        {
            drawIndex = hand.Count;
            hand.Add(newCard);
        }
        else
        {
            drawIndex = hand.FindIndex((card) =>
                card.status == Card.CardStatus.Deck);

            if (drawIndex != -1)
            {
                hand[drawIndex] = newCard;
            }
            else
            {
                Debug.LogError("Could not find card to draw");
            }

        }

        GameObject cardPrefab = gm.spellCardPrefab;
        if (newCard is CreatureCard)
        {
            cardPrefab = gm.creatureCardPrefab;
        }
        GameObject newCardObject = Instantiate(cardPrefab);
        newCardObject.GetComponent<CardManager>().card = hand[drawIndex];
        cardSlots[drawIndex].SetCard(newCardObject);
    }

    public void Draw(int amount)
    {
        for (int i = 0; i < amount; i++) {
            Draw();
        }
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
        healthText.text = health.ToString();
    }

    public Vector3Int GetStats()
    {
        return new Vector3Int(health, attack, speed);
    }

}

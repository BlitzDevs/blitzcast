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

    private GameManager.Team team;
    private int health;
    private int attack;
    private int speed;
    [SerializeField] private List<Card> playingDeck; // deck in play
    [SerializeField] private List<Card> hand;
    [SerializeField] private List<CardSlot> cardSlots;

    private GameManager gm;
    private System.Random randomGenerator = new System.Random();

    public void Initialize(GameManager.Team team)
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
            cardSlots[i].SetTeam(team);
        }

        // draw first cards
        playingDeck = Clone(deck);
        Shuffle();
        Draw(handSize);
    }

    public void Draw()
    {
        if (playingDeck.Count == 0) // Reshuffle if empty
        {
            playingDeck = Clone(deck);
            Shuffle();
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
        CardManager newCardManager = newCardObject.GetComponent<CardManager>();
        newCardManager.card = hand[drawIndex];
        cardSlots[drawIndex].SetCard(newCardObject);

        if (team == gm.userTeam)
        {
            newCardManager.ShowFront();
        }
        else
        {
            newCardManager.ShowBack();
        }
    }

    public void Draw(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Draw();
        }
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

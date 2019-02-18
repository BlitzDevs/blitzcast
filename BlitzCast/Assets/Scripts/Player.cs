using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IEntity {

    public string username;
    public GameObject cardPrefab;
    public GameObject handArea;
    public List<Card> deck; // original deck
    public List<string> enchantments;
    public int handCount = 4;

    public Text usernameText;
    public Text healthText;

    private int health;
    [SerializeField]
    private List<Card> playingDeck; // deck in play
    [SerializeField]
    private List<Card> hand;

    private System.Random randomGenerator = new System.Random();


    public void Initialize() {
        SetHealth(100);
        usernameText.text = username;
        hand = new List<Card>(handCount);

        playingDeck = CardUtilities.Clone(deck);
        CardUtilities.Shuffle(playingDeck, randomGenerator);
        Draw(handCount);
    }

    public void Draw()
    {
        Debug.Log("Entering Player Draw");
        //debug
        foreach (Card c in hand) {
            Debug.Log(c.status);
        }

        if (playingDeck.Count == 0)
        {
            Debug.Log("Reshuffle");
            playingDeck = CardUtilities.Clone(deck);
            CardUtilities.Shuffle(playingDeck, randomGenerator);
        }
        Card newCard = deck[0].Clone();
        newCard.status = Card.CardStatus.Held;
        deck.RemoveAt(0);

        //debug
        Debug.Log("newCard generated");

        int drawIndex;
        if (hand.Count < 4)
        {
            drawIndex = hand.Count;
            hand.Add(newCard);
        }
        else
        {
            drawIndex = hand.FindIndex(card => card.status == Card.CardStatus.Deck);
            if (drawIndex != -1)
                hand[drawIndex] = newCard;
            else
                Debug.LogError("Disaster in CardUtilities::Draw()");
        }

        GameObject newCardInstance = Instantiate(cardPrefab, handArea.transform);
        newCardInstance.GetComponent<CardManager>().card = hand[drawIndex];
    }

    public void Draw(int amount)
    {
        for (int i = 0; i < amount; i++)
            Draw();
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

}

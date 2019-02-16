using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public string username;
    public GameObject cardPrefab;
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
        Draw();
    }

    public void Draw()
    {
        if (playingDeck.Count == 0)
        {
            Debug.Log("Reshuffle");
            playingDeck = CardUtilities.Clone(deck);
            CardUtilities.Shuffle(playingDeck, randomGenerator);
        }
        CardUtilities.Draw(playingDeck, hand, handCount);
        for (var i = 0; i < handCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, new Vector3(200 + 400*i, 100, 0), Quaternion.identity, transform);
            newCard.GetComponent<CardManager>().card = hand[i];
        }

    }

    public void AddHealth(int hp)
    {
        health += hp;
    }

    private void SetHealth(int hp)
    {
        health = hp;
        healthText.text = health.ToString();
    }

}

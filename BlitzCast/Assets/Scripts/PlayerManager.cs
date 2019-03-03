using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IEntity
{
    public Player player;
    
    public int health;
    public List<string> enchantments;

    public Image iconImage;
    public Text usernameText;
    public Text healthText;
    public Slider healthSlider;

    private GameManager.Team team;
    [SerializeField] private List<Card> playingDeck; // deck in play
    //[SerializeField] private List<Card> heldCards;
    public List<CardSlot> cardSlots;
    public List<CreatureSlot> creatureSlots;

    private int selectedCardIndex = -1;
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
        //heldCards = new List<Card>(handSize);

        // initialize CardSlots
        if (cardSlots.Count != handSize)
        {
            Debug.LogError("Player card slots not match hand size");
        }
        for (int i = 0; i < cardSlots.Count; i++)
        {
            cardSlots[i].index = i;
            cardSlots[i].SetTeam(team);
        }
        for (int i = 0; i < creatureSlots.Count; i++)
        {
            creatureSlots[i].index = i;
            creatureSlots[i].SetTeam(team);
        }

        // draw first cards
        playingDeck = Clone(player.deck);
        Shuffle();
        for (int i = 0; i < handSize; i++)
        {
            Draw(i);
        }
    }

    public void Draw(int index)
    {
        if (index > cardSlots.Count)
        {
            Debug.LogWarning("Card draw index out of range");
            return;
        }

        if (playingDeck.Count == 0) // Reshuffle if empty
        {
            playingDeck = Clone(player.deck);
            Shuffle();
        }

        // Copy card from playingDeck, take out of playingDeck
        Card newCard = playingDeck[0].Clone();
        playingDeck.RemoveAt(0);
        newCard.SetStatus(Card.CardStatus.Held);
        newCard.SetTeam(team);

        GameObject cardPrefab = gameManager.spellCardPrefab;
        if (newCard is CreatureCard)
        {
            cardPrefab = gameManager.creatureCardPrefab;
        }

        GameObject newCardObject = Instantiate(cardPrefab);
        CardManager newCardManager = newCardObject.GetComponent<CardManager>();
        newCardManager.card = newCard;
        cardSlots[index].SetObject(newCardObject);

        if (team == gameManager.userTeam)
        {
            newCardManager.ShowFront();
        }
        else
        {
            newCardManager.ShowBack();
        }
    }


    //public void Draw()
    //{
    //    if (playingDeck.Count == 0) // Reshuffle if empty
    //    {
    //        playingDeck = Clone(player.deck);
    //        Shuffle();
    //    }

    //    // Copy card from playingDeck, take out of playingDeck
    //    Card newCard = playingDeck[0].Clone();
    //    playingDeck.RemoveAt(0);
    //    newCard.SetStatus(Card.CardStatus.Held);
    //    newCard.SetTeam(team);

    //    int drawIndex;
    //    if (heldCards.Count < cardSlots.Count)
    //    {

    //        drawIndex = heldCards.Count;
    //        heldCards.Add(newCard);
    //    }
    //    else
    //    {
    //        drawIndex = heldCards.FindIndex((card) =>
    //            !card.StatusIs(Card.CardStatus.Held));

    //        if (drawIndex != -1)
    //        {
    //            heldCards[drawIndex] = newCard;
    //        }
    //        else
    //        {
    //            Debug.LogError("Could not find card to draw");
    //        }

    //    }

    //    GameObject cardPrefab = gameManager.spellCardPrefab;
    //    if (newCard is CreatureCard)
    //    {
    //        cardPrefab = gameManager.creatureCardPrefab;
    //    }

    //    GameObject newCardObject = Instantiate(cardPrefab);
    //    CardManager newCardManager = newCardObject.GetComponent<CardManager>();
    //    newCardManager.card = heldCards[drawIndex];
    //    cardSlots[drawIndex].SetObject(newCardObject);

    //    if (team == gameManager.userTeam)
    //    {
    //        newCardManager.ShowFront();
    //    }
    //    else
    //    {
    //        newCardManager.ShowBack();
    //    }
    //}

    //public void Draw(int amount)
    //{
    //    for (int i = 0; i < amount; i++)
    //    {
    //        Draw();
    //    }
    //}

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
        healthSlider.value = (float) hp / maxHealth;
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

}

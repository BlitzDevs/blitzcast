using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IEntity
{
    public Player player;
    
    public int health;
    public List<string> enchantments;

    public HeldCardSelector heldCardSelector;

    public Image iconImage;
    public Text usernameText;
    public Text healthText;
    public Slider healthSlider;
    public GameObject cardSlotsParent;
    public GameObject creatureSlotsParent;

    private GameManager.Team team;
    [SerializeField] private List<Card> playingDeck; // deck in play

    private List<HeldCardSlot> cardSlots;
    private List<CreatureSlot> creatureSlots;

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

        // initialize CardSlots

        cardSlots = new List<HeldCardSlot>();
        for (int i = 0; i < cardSlotsParent.transform.childCount; i++)
        {
            if (cardSlotsParent.transform.GetChild(i).gameObject.GetComponent<HeldCardSlot>() != null)
            {
                HeldCardSlot newSlot = cardSlotsParent.transform.GetChild(i).gameObject.GetComponent<HeldCardSlot>();
                newSlot.index = i;
                newSlot.SetTeam(team);
                cardSlots.Add(newSlot);
            }
        }

        creatureSlots = new List<CreatureSlot>();
        for (int i = 0; i < creatureSlotsParent.transform.childCount; i++)
        {
            if (creatureSlotsParent.transform.GetChild(i).gameObject.GetComponent<CreatureSlot>() != null)
            {
                CreatureSlot newSlot = creatureSlotsParent.transform.GetChild(i).gameObject.GetComponent<CreatureSlot>();
                newSlot.index = i;
                newSlot.SetTeam(team);
                creatureSlots.Add(newSlot);
            }
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
        newCard.SetStatus(Card.Status.Held);

        GameObject cardPrefab = gameManager.spellCardPrefab;
        if (newCard is CreatureCard)
        {
            cardPrefab = gameManager.creatureCardPrefab;
        }

        GameObject newCardObject = Instantiate(cardPrefab);
        CardManager newCardManager = newCardObject.GetComponent<CardManager>();
        newCardManager.Initialize(newCard, team);
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

    public List<CreatureSlot> GetCreatureSlots()
    {
        return creatureSlots;
    }

    public bool IsUser()
    {
        return team == gameManager.userTeam;
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

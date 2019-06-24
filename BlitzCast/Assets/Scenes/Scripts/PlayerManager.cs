using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all of the displays and events related to the player.
/// The player information is determined by Player.
/// </summary>
public class PlayerManager : DetailViewable
{
    public Player player;
    public Entity entity;
    public GameManager.Team team;
    public List<Card> playingDeck; // deck in play

    // References to displayable components
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Transform handSlotParent;
    [SerializeField] private Transform statusesParent;
    [SerializeField] private SpriteSheetAnimator animator;

    private GameManager gameManager;

    /// <summary>
    /// Initialize the player with the specified team, health and handSize.
    /// </summary>
    /// <param name="team">Team.</param>
    /// <param name="health">Health.</param>
    /// <param name="handSize">Hand size.</param>
    public void Initialize(GameManager.Team team, int health, int handSize)
    {
        gameManager = FindObjectOfType<GameManager>();

        this.team = team;

        // initialize Player Entity
        entity = gameObject.AddComponent<Entity>();
        entity.Initialize(health, statusesParent);
        entity.HealthChangeEvent += SetHealthDisplay;
        entity.SpeedChangeEvent += SetSpeedDisplay;

        // initialize animator
        animator.Initialize(
            player.caster.name,
            "Casters",
            player.caster.spriteAnimateSpeed,
            entity
        );
        // set text
        usernameText.text = player.username;

        CloneDeck();
        Shuffle();

        // create and initialize HandSlots
        for (int i = 0; i < handSize; i++)
        {
            GameObject handSlotObject = Instantiate(gameManager.handSlotPrefab, handSlotParent);
            HandSlot handSlot = handSlotObject.GetComponent<HandSlot>();
            handSlot.Initialize(this);
        }
    }

    /// <summary>
    /// Removes the first card from the playing deck and returns it.
    /// </summary>
    /// <returns>The removed card.</returns>
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

    /// <summary>
    /// Copy the original deck to playing deck.
    /// </summary>
    public void CloneDeck()
    {
        playingDeck = new List<Card>(player.deck.Count);

        foreach (Card c in player.deck)
        {
            playingDeck.Add(c);
        }
    }

    /// <summary>
    /// Shuffle the playing deck.
    /// </summary>
    public void Shuffle()
    {
        for (int i = 0; i < playingDeck.Count; i++)
        {
            int j = Random.Range(0, playingDeck.Count);
            var temp = playingDeck[i];
            playingDeck[i] = playingDeck[j];
            playingDeck[j] = temp;
        }
    }

    /// <summary>
    /// Added onto event OnHealthChange of entity.
    /// Sets the health display.
    /// </summary>
    public void SetHealthDisplay(int oldHP, int newHP)
    {
        healthText.text = newHP.ToString();
        healthSlider.value = (float)newHP / entity.MaxHealth;
    }

    /// <summary>
    /// Added onto event OnSpeedChange of entity.
    /// Sets the speed display.
    /// </summary>
    public void SetSpeedDisplay(float oldSpeed, float newSpeed)
    {
        speedText.text = "Speed: x" + newSpeed.ToString();
        // TODO: set color depending on slow/fast?
    }

}


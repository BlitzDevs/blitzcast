using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public Player player;
    public Entity entity;
    public GameManager.Team team;

    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Highlightable highlightable;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject handSlotPrefab;
    [SerializeField] private Transform handSlotParent;
    [SerializeField] private Transform statusesParent;
    [SerializeField] private SpriteSheetAnimator animator;
    [SerializeField] private List<Card> playingDeck; // deck in play

    private GameManager gameManager;


    public void Initialize(GameManager.Team team, int health, int handSize)
    {
        gameManager = FindObjectOfType<GameManager>();

        this.team = team;

        // initialize Player Entity
        entity = gameObject.AddComponent<Entity>();
        entity.Initialize(health, statusesParent);
        entity.HealthChangeEvent += SetHealthDisplay;
        entity.SpeedChangeEvent += SetSpeedDisplay;

        // initialize animation
        SpriteSheetAnimator.Animatable anim = new SpriteSheetAnimator.Animatable(
            player.caster.name,
            "Casters",
            player.caster.spriteAnimateSpeed,
            entity
        );
        animator.Initialize(anim);
        // set text
        usernameText.text = player.username;

        CloneDeck();
        Shuffle();

        // create and initialize HandSlots
        for (int i = 0; i < handSize; i++)
        {
            GameObject handSlotObject = Instantiate(handSlotPrefab, handSlotParent);
            HandSlot handSlot = handSlotObject.GetComponent<HandSlot>();
            handSlot.Initialize(this);
        }
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
            int j = Random.Range(0, playingDeck.Count);
            var temp = playingDeck[i];
            playingDeck[i] = playingDeck[j];
            playingDeck[j] = temp;
        }
    }

    // added onto event OnHealthChange of entity
    public void SetHealthDisplay(int oldHP, int newHP)
    {
        healthText.text = newHP.ToString();
        healthSlider.value = (float)newHP / entity.MaxHealth;
    }

    // added onto event OnSpeedChange of entity
    public void SetSpeedDisplay(float s)
    {
        speedText.text = "Speed: x" + s.ToString();
        // set color?
    }

}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Mirror;

/// <summary>
/// Handles all of the displays and events related to the player.
/// The player information is determined by Player.
/// </summary>
public class PlayerManager : NetworkBehaviour
{
    public Player player;
    public Entity entity;
    public GameManager.Team team;
    public List<Card> playingDeck; // deck in play
    public GameObject focusedObject;

    // References to displayable components
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Transform handSlotParent;
    [SerializeField] private Transform statusesParent;
    [SerializeField] private Image spriteImage;
    [SerializeField] private SpriteSheetAnimator animator;

    private GameManager gameManager;
    private EventSystem eventSystem;
    private GraphicRaycaster raycaster;

    /// <summary>
    /// Initialize the player with the specified team, health and handSize.
    /// </summary>
    /// <param name="team">Team.</param>
    /// <param name="health">Health.</param>
    /// <param name="handSize">Hand size.</param>
    public void Initialize(GameManager.Team team, int health, int handSize)
    {
        gameManager = FindObjectOfType<GameManager>();
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        this.player = player;
        this.team = team;

        // initialize Player Entity
        entity = gameObject.AddComponent<Entity>();
        entity.Initialize(health, statusesParent);
        entity.HealthChangeEvent += SetHealthDisplay;
        entity.SpeedChangeEvent += SetSpeedDisplay;

        // initialize animator
        animator.enabled = true;
        spriteImage.color = player.color;
        animator.Initialize(
            player.caster.name,
            "Casters",
            player.caster.spriteAnimateSpeed,
            entity
        );
        // set text
        usernameText.text = player.username;

        // only initialize cards if local
        if (team == GameManager.Team.Friendly)
        {
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
    /// Gets all GameObjects under cursor.
    /// </summary>
    public List<GameObject> GetAllUnderCursor()
    {
        List<GameObject> results = new List<GameObject>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> rayCast = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, rayCast);

        foreach (RaycastResult r in rayCast)
        {
            results.Add(r.gameObject);
        }
        return results;
    }

    /// <summary>
    /// Gets the first GameObject under cursor with the specified component.
    /// </summary>
    public GameObject GetFirstUnderCursor<T>()
    {
        List<GameObject> hitObjects = GetAllUnderCursor();
        foreach (GameObject g in hitObjects)
        {
            T t = g.GetComponent<T>();
            if (t != null)
            {
                return g;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the first GameObject under cursor in the specified layer.
    /// </summary>
    public GameObject GetFirstUnderCursor(int layer)
    {
        List<GameObject> hitObjects = GetAllUnderCursor();
        foreach (GameObject g in hitObjects)
        {
            if (g.layer == layer)
            {
                return g;
            }
        }
        return null;
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


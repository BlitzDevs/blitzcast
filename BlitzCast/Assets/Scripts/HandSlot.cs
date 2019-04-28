using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Handslot, mostly independent of player object
/// Has references to card it contains and manages redraw time
/// </summary>
public class HandSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // whatever card is in the hand
    public CardManager cardInSlot;
    // visual representation of redraw
    private CircleTimer drawTimer;

    // references to useful objects
    private GameManager gameManager;
    private EventSystem eventSystem;
    private PlayerManager player;


    /// <summary>
    /// called by PlayerManager when PlayerManager Initializes
    /// </summary>
    /// <param name="player">
    /// player passes itself so slot knows what to reference
    /// </param>
    public void Initialize(PlayerManager player)
    {
        //works because there is only one in hierarchy
        eventSystem = FindObjectOfType<EventSystem>();
        gameManager = FindObjectOfType<GameManager>();
        //initialize reference to player
        this.player = player;
        
        //initialize drawTimer at the location of the slot (center)
        drawTimer = gameManager.NewCircleTimer(transform);
        //reference to player's entity to scale redraw time 
        drawTimer.entity = player.entity;

        //fills the slot
        DrawCard();
    }

    /// <summary>
    /// Timer to determine cooldown of the card
    /// </summary>
    /// <param name="time">
    /// measured in seconds but can be scaled by speed
    /// </param>
    public void StartDrawTimer(float time)
    {
        StartCoroutine(DrawTimer(time));
    }

    /// <summary>
    /// Handles visuals for the DrawTimer delay
    /// </summary>
    /// <param name="time">
    /// Time measured in seconds but can be scaled by speed in Entity
    /// </param>
    /// <returns>
    /// Lets it work with Coroutines as a delay
    /// </returns>
    private IEnumerator DrawTimer(float time)
    {
        //make the timer appear
        drawTimer.gameObject.SetActive(true);
        //So we can scale the time based on player's entity
        drawTimer.entity = player.entity;
        drawTimer.StartTimer(time);
        while (!drawTimer.IsComplete())
        {
            yield return null;
        }
        //make timer dissappear
        drawTimer.gameObject.SetActive(false);
        //refill the slot
        DrawCard();
    }

    /// <summary>
    /// fills the slot based on player's Deck Object
    /// </summary>
    private void DrawCard()
    {
        Card card = player.DrawTop();
        //prefab determines how properties are displayed
        GameObject cardPrefab = null;
        if (card is CreatureCard)
        {
            cardPrefab = gameManager.creatureCardPrefab;
        } else if (card is SpellCard)
        {
            cardPrefab = gameManager.spellCardPrefab;
        } else
        {
            Debug.LogError("Card type is unknown");
        }

        GameObject cardObject = Instantiate(cardPrefab);
        //define slotObject which is actually a CardManager
        cardInSlot = cardObject.GetComponent<CardManager>();
        //initialize slotObject with some properties
        cardInSlot.Initialize(card, this, player);

        //move the slotObject to the handSlot and scale it correctly
        cardInSlot.transform.SetParent(transform);
        cardInSlot.transform.localPosition = Vector3.zero;
        cardInSlot.transform.localScale = Vector3.one;
        cardInSlot.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// lets eventSystem know the CardManager is being selected
    /// </summary>
    /// <param name="eventData">
    /// pointer info
    /// </param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardInSlot != null)
        {
            eventSystem.SetSelectedGameObject(cardInSlot.gameObject);
        }
    }

    /// <summary>
    /// lets eventSystem know CardManager not being selected anymore
    /// </summary>
    /// <param name="eventData">
    /// pointer info
    /// </param>
    public void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
    }

}

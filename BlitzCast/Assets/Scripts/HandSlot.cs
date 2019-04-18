using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handslot, mostly independent of player object
/// Has references to card it contains and manages redraw time
/// </summary>
public class HandSlot : Selectable, IDeselectHandler, ISelectHandler,
                            IPointerEnterHandler, IPointerExitHandler
{
    //whatever card is in the hand
    public CardManager slotObject;
    //visual representation of redraw
    private CircleTimer drawTimer;
    //if cast fails/mouse released card returns here
    private Vector2 originalPosition = Vector2.zero;
    //card bobs upwards by this many pixels when hovered
    private const int pixelsToFloatWhenSelected = 20;

    private EventSystem eventSystem;
    //reference to player
    private PlayerManager player;
    //reference to gameManager
    private GameManager gameManager;


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
        slotObject = cardObject.GetComponent<CardManager>();
        //initialize slotObject with some properties
        slotObject.Initialize(card, this, player);

        //move the slotObject to the handSlot and scale it correctly
        slotObject.transform.SetParent(transform);
        slotObject.transform.localPosition = Vector3.zero;
        slotObject.transform.localScale = Vector3.one;
        slotObject.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// lets eventSystem know the CardManager is being selected
    /// </summary>
    /// <param name="eventData">
    /// pointer info
    /// </param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }

    /// <summary>
    /// lets eventSystem know CardManager not being selected anymore
    /// </summary>
    /// <param name="eventData">
    /// pointer info
    /// </param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
    }

    /// <summary>
    /// peeks card when selected
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnSelect(BaseEventData eventData)
    {
        if (slotObject != null)
        {
            //Float
            //slotObject.transform.localPosition = new Vector3(
            //originalPosition.x, originalPosition.y + pixelsToFloatWhenSelected, 0);

            //slides card upwards
            slotObject.cardMover.SetPosition(new Vector3(
                originalPosition.x, originalPosition.y + pixelsToFloatWhenSelected, 0));
        }
    }

    /// <summary>
    /// unpeeks card when deselected
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDeselect(BaseEventData eventData)
    {
        if (slotObject != null)
        {
            //Unfloat
            //slotObject.transform.localPosition = originalPosition;

            //moves card back
            slotObject.cardMover.SetPosition(originalPosition);
        }
    }

}

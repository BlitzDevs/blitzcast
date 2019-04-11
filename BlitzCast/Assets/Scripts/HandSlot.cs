using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class HandSlot : Selectable, IDeselectHandler, ISelectHandler,
                            IPointerEnterHandler, IPointerExitHandler
{

    public GameObject slotObject;

    private CircleTimer drawTimer;
    private Vector2 originalPosition = Vector2.zero;
    private const int pixelsToFloatWhenSelected = 20;
    private EventSystem eventSystem;
    private PlayerManager player;
    private GameManager gameManager;


    // Called by Player
    public void Initialize(PlayerManager player)
    {
        eventSystem = FindObjectOfType<EventSystem>();
        gameManager = FindObjectOfType<GameManager>();
        this.player = player;

        drawTimer = gameManager.NewCircleTimer(transform);

        DrawCard();
    }

    public void StartDrawTimer(float time)
    {
        StartCoroutine(DrawTimer(time));
    }

    private IEnumerator DrawTimer(float time)
    {
        drawTimer.gameObject.SetActive(true);
        drawTimer.StartTimer(time);
        while (!drawTimer.IsComplete())
        {
            yield return null;
        }
        drawTimer.gameObject.SetActive(false);
        DrawCard();
    }

    private void DrawCard()
    {
        Card card = player.DrawTop();
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
        cardObject.GetComponent<CardManager>().Initialize(card, player.team, this);

        slotObject = cardObject;
        slotObject.transform.SetParent(transform);
        slotObject.transform.localScale = Vector3.one;
        slotObject.transform.localPosition = Vector3.zero;
        slotObject.transform.localRotation = Quaternion.identity;
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (slotObject != null)
        {
            // Float
            slotObject.transform.localPosition = new Vector3(
                originalPosition.x, originalPosition.y + pixelsToFloatWhenSelected, 0);
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (slotObject != null)
        {
            // Unfloat
            slotObject.transform.localPosition = originalPosition;
        }
    }

}

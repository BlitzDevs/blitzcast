using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandSlot : Selectable, IDeselectHandler, ISelectHandler,
                            IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public GameObject slotObject;
    private Vector2 originalPosition = Vector2.zero;
    private const int pixelsToFloatWhenSelected = 20;
    private EventSystem eventSystem;
    private PlayerManager player;

    public void Initialize(PlayerManager player)
    {
        eventSystem = FindObjectOfType<EventSystem>();
        this.player = player;
        AssignCard(this.player.DrawTop());

    }

    public void AssignCard(Card card)
    {
        GameObject cardPrefab = null;
        if (card is CreatureCard)
        {
            cardPrefab = player.creatureCardPrefab;
        } else if (card is SpellCard)
        {
            cardPrefab = player.spellCardPrefab;
        } else
        {
            Debug.LogError("Card is not CreatureCard or SpellCard >:(");
        }

        GameObject cardObject = Instantiate(cardPrefab);
        cardObject.GetComponent<CardManager>().Initialize(card, player.team);
        SetObject(cardObject);
    }

    private void SetObject(GameObject newObject)
    {
        slotObject = newObject;
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
        // Float
        slotObject.transform.localPosition = new Vector3(
            originalPosition.x, originalPosition.y + pixelsToFloatWhenSelected, 0);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        // Unfloat
        slotObject.transform.localPosition = originalPosition;
    }
}

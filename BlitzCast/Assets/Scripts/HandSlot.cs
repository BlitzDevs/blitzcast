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

    private void SetObject(GameObject slotObject)
    {
        this.slotObject = slotObject;
        this.slotObject.transform.SetParent(this.transform);
        this.slotObject.transform.localScale = Vector3.one;
        this.slotObject.transform.localPosition = Vector3.zero;
        this.slotObject.transform.localRotation = Quaternion.identity;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(this.gameObject);
        Debug.Log("Pointer enter");
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
        Debug.Log("Pointer exit");
    }

    public override void OnSelect(BaseEventData eventData)
    {
        Float();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        Unfloat();
    }

    public void Float()
    {
        slotObject.transform.localPosition = new Vector3(
            originalPosition.x, originalPosition.y + pixelsToFloatWhenSelected, 0);
    }

    public void Unfloat()
    {
        slotObject.transform.localPosition = originalPosition;
    }
}

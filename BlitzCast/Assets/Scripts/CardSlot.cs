using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour,
                        IPointerEnterHandler, IPointerExitHandler {

    public int index;
    public GameObject cardObject;

    private Card card;
    private Vector2 originalPosition = new Vector2();
    private const int pixelsToFloatWhenSelected = 20;

    // TODO: (Future) Selectable with keys
    // TODO: Fix ordering, not float when dragging
    public void OnPointerEnter(PointerEventData eventData)
    {
        Float();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unfloat();
    }

    public void Float()
    {
        if (card.status == Card.CardStatus.Held)
        {
            originalPosition = cardObject.transform.localPosition;
            cardObject.transform.localPosition = new Vector3(
                originalPosition.x,
                originalPosition.y + pixelsToFloatWhenSelected,
                0);
        }
    }

    public void Unfloat()
    {
        if (card.status == Card.CardStatus.Held)
        {
            cardObject.transform.localPosition = originalPosition;
        }
    }

    public void SetCard(GameObject cardObject)
    {
        this.cardObject = cardObject;
        this.card = cardObject.GetComponent<CardManager>().card;
        this.cardObject.transform.SetParent(this.transform);
        this.cardObject.transform.localScale = Vector3.one;
        this.cardObject.transform.localPosition = Vector3.zero;
        this.cardObject.transform.localRotation = Quaternion.identity;
    }

    public void RemoveCard()
    {
        this.card = null;
    }
}

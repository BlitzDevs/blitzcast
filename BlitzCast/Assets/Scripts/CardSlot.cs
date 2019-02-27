using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : Slot, IPointerEnterHandler, IPointerExitHandler
{

    private Vector2 originalPosition = new Vector2();
    private const int pixelsToFloatWhenSelected = 20;


    // TODO: (Future) Selectable with keys
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card.StatusIs(Card.CardStatus.Held))
        {
            Float();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (card.StatusIs(Card.CardStatus.Held))
        {
            Unfloat();
        }
    }



    private void Float()
    {
        if (team == userTeam && card.StatusIs(Card.CardStatus.Held))
        {
            originalPosition = slotObject.transform.localPosition;
            slotObject.transform.localPosition = new Vector3(
                originalPosition.x,
                originalPosition.y + pixelsToFloatWhenSelected,
                0);
        }
    }

    private void Unfloat()
    {
        if (team == userTeam && card.StatusIs(Card.CardStatus.Held))
        {
            slotObject.transform.localPosition = originalPosition;
        }
    }

}

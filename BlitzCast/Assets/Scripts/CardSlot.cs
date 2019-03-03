using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : Slot, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject redrawDisplay;
    public Slider drawTimeSlider;

    private Card card;
    private Vector2 originalPosition = new Vector2();
    private const int pixelsToFloatWhenSelected = 20;
    private PlayerManager player;

    private void Start()
    {
        redrawDisplay.SetActive(false);
        player = FindObjectOfType<GameManager>().GetPlayer(team);
    }

    public IEnumerator DrawCard(int drawTimeout)
    {
        redrawDisplay.SetActive(true);

        float timer = 0;
        while (timer < drawTimeout)
        {
            timer += Time.deltaTime;
            drawTimeSlider.value = timer / drawTimeout;

            yield return null;
        }

        redrawDisplay.SetActive(false);
        player.Draw(index);
        
    }


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
        if (card.StatusIs(Card.CardStatus.Held))
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
        if (card.StatusIs(Card.CardStatus.Held))
        {
            slotObject.transform.localPosition = originalPosition;
        }
    }

    public override void SetObject(GameObject slotObject)
    {
        card = slotObject.GetComponent<CardManager>().card;
        this.slotObject = slotObject;
        this.slotObject.transform.SetParent(this.transform);
        this.slotObject.transform.localScale = Vector3.one;
        this.slotObject.transform.localPosition = Vector3.zero;
        this.slotObject.transform.localRotation = Quaternion.identity;
    }

}

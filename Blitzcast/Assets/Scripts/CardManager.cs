using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;
    public Text nameText;
    public Text descriptionText;
    public Image artImage;
    public Text timeText;

    private Vector2 originalPosition = new Vector2();
    private Vector2 dragOffset = new Vector2();

    // Use this for initialization
    void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;
        artImage.sprite = card.art;
        timeText.text = card.timeCost.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = this.transform.position;
        dragOffset = originalPosition - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.position = originalPosition;
    }

}

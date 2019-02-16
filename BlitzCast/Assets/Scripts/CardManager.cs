using System.Collections;
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
    public Image cardBack;
    public Image tint;

    private GameManager gm;
    private Vector2 originalPosition = new Vector2();
    private Vector2 dragOffset = new Vector2();

    // Use this for initialization
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        nameText.text = card.name;
        descriptionText.text = card.description;
        artImage.sprite = card.art;
        timeText.text = card.timeCost.ToString();
        tint.color = Color.clear;
        cardBack.enabled = false;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held)
        {
            originalPosition = transform.position;
            dragOffset = originalPosition - eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held)
            transform.position = eventData.position + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held)
        {
            transform.position = originalPosition;

            StartCoroutine(PlayCard());
        }


    }


    IEnumerator PlayCard()
    {
        Debug.Log("Casting " + card.name);

        card.status = Card.CardStatus.Casting;
        tint.color = new Color32(255, 255, 255, 100);

        for (var i = card.timeCost; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            timeText.text = i.ToString();
        }

        gm.Cast(card);
        card.status = Card.CardStatus.Recharging;
        tint.color = new Color32(0, 0, 0, 100);

        for (var i = card.timeCost; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            timeText.text = i.ToString();
        }

        card.status = Card.CardStatus.Deck;
        gm.player.Draw();
        Destroy(this.gameObject);
        
    }
}

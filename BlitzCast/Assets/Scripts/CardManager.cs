using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour,
                           IPointerEnterHandler, IPointerExitHandler,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;
    public Text nameText;
    public Text descriptionText;
    public Image artImage;
    public Text timeText;
    public Image cardBack;
    public Image tint;

    private GameManager gm;
    private RaycastTargeter targeter;
    private Vector2 originalPosition = new Vector2();
    private Vector2 dragOffset = new Vector2();
    private const int hoverMovePixels = 50;

    // Use this for initialization
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        targeter = FindObjectOfType<RaycastTargeter>();

        nameText.text = card.name;
        descriptionText.text = card.description;
        artImage.sprite = card.art;
        timeText.text = card.timeCost.ToString();
        tint.color = Color.clear;
        cardBack.enabled = false;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("entering " + card.name);
        if (card.status == Card.CardStatus.Held)
        {
            originalPosition = transform.position;
            transform.position = new Vector3(
                originalPosition.x,
                originalPosition.y + hoverMovePixels,
                0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("exiting " + card.name);
        if (card.status == Card.CardStatus.Held)
            transform.position = originalPosition;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held)
            dragOffset = originalPosition - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held)
            transform.position = eventData.position + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held && targeter.inCastZone())
        {
            StartCoroutine(PlayCard());
        }
        else
        {
            transform.position = originalPosition;
        }


    }


    IEnumerator PlayCard()
    {
        GameObject target = targeter.GetTarget();
        transform.position = target.transform.position;

        Debug.Log("Casting " + card.name + " on " + target.name);

        card.status = Card.CardStatus.Casting;
        tint.color = new Color32(255, 255, 255, 100);

        for (var i = card.timeCost; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            timeText.text = i.ToString();
        }

        gm.Cast(card, target);
        card.status = Card.CardStatus.Recharging;
        tint.color = new Color32(0, 0, 0, 100);

        for (var i = card.timeCost; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            timeText.text = i.ToString();
        }

        card.status = Card.CardStatus.Deck;
        gm.player.Draw();
        //Destroy(this.gameObject);
        
    }

}

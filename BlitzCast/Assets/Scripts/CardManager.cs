using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;
    public Text nameText;
    public Text descriptionText;
    public Image artImage;
    public Text castTimeText;
    public Text redrawTimeText;
    public Image cardBack;
    public Image tint;

    private GameManager gm;
    private Vector2 originalPosition;
    private Vector2 dragOffset;

    // Use this for initialization
    void Start()
    {
        gm = FindObjectOfType<GameManager>();

        nameText.text = card.name;
        descriptionText.text = card.description;
        artImage.sprite = card.art;
        castTimeText.text = card.castTime.ToString();
        redrawTimeText.text = card.redrawTime.ToString();
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
        if (card.status == Card.CardStatus.Held && gm.targeter.InCastZone())
        {
            StartCoroutine(PlayCard());
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }


    }


    IEnumerator PlayCard() //TODO: Move into GameManager
    {
        GameObject target = gm.targeter.GetTarget();
        if (target != null)
        {
            transform.SetParent(gm.targeter.GetCastingSlot());
        }

        Debug.Log("Casting " + card.name + " on " + target.name);

        card.status = Card.CardStatus.Casting;
        tint.color = new Color32(255, 255, 255, 100);

        for (var i = card.castTime; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            castTimeText.text = i.ToString();
        }

        gm.Cast(this.gameObject, target);
        card.status = Card.CardStatus.Recharging;
        tint.color = new Color32(0, 0, 0, 100);

        for (var i = card.redrawTime; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            redrawTimeText.text = i.ToString();
        }

        card.status = Card.CardStatus.Deck;
        card.GetOwner().Draw();
        Destroy(this.gameObject);

    }

}

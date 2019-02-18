using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;
    public bool showingFront;

    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image artImage;
    [SerializeField] private Text castTimeText;
    [SerializeField] private Text redrawTimeText;
    [SerializeField] private GameObject cardFront;
    [SerializeField] private GameObject cardBack;
    [SerializeField] private Image tint;

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
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held && showingFront)
        {
            originalPosition = transform.position;
            dragOffset = originalPosition - eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held && showingFront)
            transform.position = eventData.position + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (card.status == Card.CardStatus.Held && showingFront &&
            gm.targeter.InCastZone())
        {
            StartCoroutine(PlayCard());
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }


    public void ShowFront()
    {
        showingFront = true;
        cardBack.SetActive(false);
        cardFront.SetActive(true);
    }

    public void ShowBack()
    {
        showingFront = false;
        cardBack.SetActive(true);
        cardFront.SetActive(false);
    }


    IEnumerator PlayCard() //TODO: Move into GameManager
    {
        GameObject target = gm.targeter.GetTarget();
        if (target != null)
        {
            transform.SetParent(gm.targeter.GetCastingSlot());
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
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

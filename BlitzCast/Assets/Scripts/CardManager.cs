using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;

    [SerializeField] private Text nameText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image artImage;
    [SerializeField] private Text castTimeText;
    [SerializeField] private Text redrawTimeText;
    [SerializeField] private GameObject cardFront;
    [SerializeField] private GameObject cardBack;
    [SerializeField] private GameObject targetableZone;
    [SerializeField] private Image tint;

    private GameManager gm;
    private Vector2 originalPosition;
    private Vector2 dragOffset;

    // Use this for initialization
    void Start()
    {
        gm = FindObjectOfType<GameManager>();

        nameText.text = card.cardName;
        typeText.text = card.behavior.action.ToString();
        descriptionText.text = card.description;
        artImage.sprite = card.art;
        castTimeText.text = card.castTime.ToString();
        redrawTimeText.text = card.redrawTime.ToString();
        tint.color = Color.clear;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card.StatusIs(Card.CardStatus.Held))
        {
            card.SetStatus(Card.CardStatus.Dragging);
            originalPosition = transform.position;
            dragOffset = originalPosition - eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        card.SetStatus(Card.CardStatus.Held);
        CastZone castZone = gm.targeter.GetTargetCastZone(
            card.behavior.action, targetableZone);
        if (castZone != null)
        {
            StartCoroutine(gm.CastCard(this.gameObject, castZone));
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }


    public void ShowFront()
    {
        cardBack.SetActive(false);
        cardFront.SetActive(true);
    }

    public void ShowBack()
    {
        cardBack.SetActive(true);
        cardFront.SetActive(false);
    }

    public void CastingAnimation()
    {
        // temporary
        tint.color = new Color32(255, 255, 255, 100);
    }

    public void RechargingAnimation()
    {
        // temporary
        tint.color = new Color32(0, 0, 0, 100);
    }

    public void SetCastTimer(int time)
    {
        // add bar or something instead of just text
        castTimeText.text = time.ToString();
    }

    public void SetRedrawTimer(int time)
    {
        // add bar or something instead of just text
        redrawTimeText.text = time.ToString();
    }

    void OnDisable()
    {
        Debug.Log("CardManager was disabled");
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        targetableZone.SetActive(false);
    }

}

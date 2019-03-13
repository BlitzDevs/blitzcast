using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField] private Text nameText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image artImage;
    [SerializeField] private Text castTimeText;
    [SerializeField] private GameObject castSliderObject;
    [SerializeField] private Slider castSlider;
    [SerializeField] private Text redrawTimeText;
    [SerializeField] private GameObject cardFront;
    [SerializeField] private GameObject cardBack;
    [SerializeField] private GameObject targetableZone;
    [SerializeField] private Image tint;

    private Card card;
    private GameManager.Team team;

    private GameManager gameManager;
    private Vector2 originalPosition;
    private Vector2 dragOffset;


    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        nameText.text = card.cardName;
        typeText.text = card.behaviors[0].action.ToString();
        descriptionText.text = card.description;
        artImage.sprite = card.art;
        castTimeText.text = card.castTime.ToString();
        redrawTimeText.text = card.redrawTime.ToString();
        tint.color = Color.clear;
        castSliderObject.SetActive(false);
    }

    // Called by Player Manager
    public void Initialize(Card card, GameManager.Team team)
    {
        this.card = card;
        this.team = team;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card.StatusIs(Card.Status.Held))
        {
            card.SetStatus(Card.Status.Dragging);
            originalPosition = transform.position;
            dragOffset = originalPosition - eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position + dragOffset;
        transform.position = new Vector2(
            Mathf.RoundToInt(eventData.position.x + dragOffset.x),
            Mathf.RoundToInt(eventData.position.y + dragOffset.y));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        card.SetStatus(Card.Status.Held);

        bool castedCard = TryCastCard();
        if (!castedCard) {
            transform.localPosition = Vector3.zero;
        }
    }

    public bool TryCastCard()
    {
        CastZone castZone = gameManager.targeter.GetTargetCastZone(
            card.behaviors, targetableZone);
        if (castZone != null)
        {
            // Pass the card slot
            StartCoroutine(gameManager.CastCard(
                gameObject.GetComponentInParent<HeldCardSlot>(), castZone));
            return true;
        }
        return false;
    }


    public Card GetCard()
    {
        return card;
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
        castSliderObject.SetActive(true);
        tint.color = new Color32(255, 255, 255, 100);
    }

    public void StopCastingAnimation()
    {
        // temporary
        castSliderObject.SetActive(false);
    }

    public void SetCastTimer(float time)
    {
        castTimeText.text = Mathf.Round(time).ToString();
        castSlider.value = time / card.castTime;
    }


    void OnDisable()
    {
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        targetableZone.SetActive(false);
        StopCastingAnimation();
    }

}

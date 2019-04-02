using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField] protected Text nameText;
    [SerializeField] protected Text raceText;
    [SerializeField] protected Image artImage;
    [SerializeField] protected Text castTimeText;
    [SerializeField] protected GameObject castSliderObject;
    [SerializeField] protected Slider castSlider;
    [SerializeField] protected Text redrawTimeText;
    [SerializeField] protected GameObject cardFront;
    [SerializeField] protected GameObject cardBack;
    [SerializeField] protected GameObject targetableZone;
    [SerializeField] protected Image tint;

    protected Card card;
    protected GameManager.Team team;

    protected GameManager gameManager;
    protected Vector2 originalPosition;
    protected Vector2 dragOffset;

    abstract public void Cast(List<GameObject> Targets);

    protected void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        nameText.text = card.cardName;
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
        if (gameObject.layer == LayerMask.GetMask("Held")) {
            gameObject.layer = LayerMask.GetMask("Active");
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
        bool castedCard = TryCastCard();
        if (!castedCard) {
            transform.localPosition = Vector3.zero;
        }
    }

    public bool TryCastCard()
    {
        //TODO: :D
        return false;
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

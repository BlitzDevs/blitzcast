using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected GameObject cardFront;
    [SerializeField] protected GameObject cardBack;
    [SerializeField] protected GameObject targetableZone;
    [SerializeField] protected GameObject castSliderObject;
    [SerializeField] protected Slider castSlider;
    [SerializeField] protected Image artImage;
    [SerializeField] protected Image tint;
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text raceText;
    [SerializeField] protected TMP_Text redrawTimeText;
    [SerializeField] protected TMP_Text castTimeText;

    public Card card;
    public GameManager.Team team;

    protected GameManager gameManager;
    protected CreatureGrid grid;
    protected HandSlot slot;

    private Vector2 dragOffset;


    // Initialize is called by HandSlot
    abstract public void Initialize(Card card, GameManager.Team team, HandSlot slot);
    abstract public void Cast(GameObject target);
    abstract public void EnablePreview(GameObject target);
    abstract public void DisablePreview();
    abstract public GameObject GetCastLocation();
    abstract public void DestroySelf();


    protected void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        grid = FindObjectOfType<CreatureGrid>();
        
        nameText.text = card.cardName;
        artImage.sprite = card.art;
        castTimeText.text = card.castTime.ToString();
        redrawTimeText.text = card.redrawTime.ToString();
        tint.color = Color.clear;
        castSliderObject.SetActive(false);
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameObject.layer == SortingLayer.GetLayerValueFromName("Held")) {
            gameObject.layer = SortingLayer.GetLayerValueFromName("Active");
            dragOffset = (Vector2) transform.position - eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position + dragOffset;
        transform.position = new Vector2(
            Mathf.RoundToInt(eventData.position.x + dragOffset.x),
            Mathf.RoundToInt(eventData.position.y + dragOffset.y));

        GameObject target = GetCastLocation();
        if (target != null)
        {
            EnablePreview(target);
        }
        else
        {
            DisablePreview();
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = GetCastLocation();
        if (target != null)
        {
            StartCoroutine(CastTimer(target));
        }
        else
        {
            gameObject.layer = SortingLayer.GetLayerValueFromName("Held");
            DisablePreview();
            transform.localPosition = Vector3.zero;
        }
    }

    private IEnumerator CastTimer(GameObject target)
    {
        // remove self from card slot
        slot.slotObject = null;
        // temporary countdown
        yield return new WaitForSeconds(card.castTime);
        // start card draw timer after casted
        slot.StartCardDrawTimer(card.redrawTime);
        // cast
        Cast(target);
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
    }

}

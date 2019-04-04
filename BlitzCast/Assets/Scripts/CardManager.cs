using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // SeralizeField tells the Unity editor to display variable even if private
    // protected is a private variable accessible by inheriting members
    [SerializeField] protected GameObject cardFront;
    [SerializeField] protected GameObject cardBack;
    [SerializeField] protected GameObject targetableZone;
    [SerializeField] protected GameObject castSliderObject;
    [SerializeField] protected Slider castSlider;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Image sprite;
    [SerializeField] protected Image artImage;
    [SerializeField] protected Image tint;
    // TMP_Text is TextMeshPro text; much better than default Unity text
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


    // abstract functions are to be implemented by inherting classes
    abstract public void Cast(GameObject target);
    abstract public void EnablePreview(GameObject target);
    abstract public void DisablePreview();
    abstract public GameObject GetCastLocation();
    abstract public void DestroySelf();

    // virtual functions are overrideable but can have a body
    // Initialize is called by HandSlot
    public virtual void Initialize(
        Card card, GameManager.Team team, HandSlot slot)
    {
        this.card = card;
        this.team = team;
        this.slot = slot;

        nameText.text = card.cardName;
        artImage.sprite = card.art;
        castTimeText.text = card.castTime.ToString();
        redrawTimeText.text = card.redrawTime.ToString();
        animator.runtimeAnimatorController = card.animator;

        castSliderObject.SetActive(false);
    }

    // Start is called by Unity on first time this object is active
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        grid = FindObjectOfType<CreatureGrid>();
    }

    // When begin dragging card, move card to Active layer
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameObject.layer == SortingLayer.GetLayerValueFromName("Held")) {
            gameObject.layer = SortingLayer.GetLayerValueFromName("Active");
            dragOffset = (Vector2) transform.position - eventData.position;
        }
    }

    // While dragging, move the card and try preview
    public void OnDrag(PointerEventData eventData)
    {
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

    // When stop dragging, start casting if valid; else return to hand
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

}

using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : Slot, IDeselectHandler, ISelectHandler,
                            IPointerEnterHandler, IPointerExitHandler
{
    public Card card;

    private Vector2 originalPosition = Vector2.zero;
    private const int pixelsToFloatWhenSelected = 20;
    protected EventSystem eventSystem;

    protected override void Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(this.gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        Float();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        Unfloat();
    }

    public void Float()
    {
        slotObject.transform.localPosition = new Vector3(
            originalPosition.x, originalPosition.y + pixelsToFloatWhenSelected, 0);
    }

    public void Unfloat()
    {
        slotObject.transform.localPosition = originalPosition;
    }


    public override void SetObject(GameObject slotObject)
    {
        card = slotObject.GetComponent<CardManager>().GetCard();
        this.slotObject = slotObject;
        this.slotObject.transform.SetParent(this.transform);
        this.slotObject.transform.localScale = Vector3.one;
        this.slotObject.transform.localPosition = Vector3.zero;
        this.slotObject.transform.localRotation = Quaternion.identity;
    }

}

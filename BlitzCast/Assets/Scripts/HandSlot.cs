using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandSlot : Selectable, IDeselectHandler, ISelectHandler,
                            IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public GameObject slotObject;
    private Vector2 originalPosition = Vector2.zero;
    private const int pixelsToFloatWhenSelected = 20;
    private EventSystem eventSystem;

    protected override void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }

    public void SetObject(GameObject slotObject)
    {
        this.slotObject = slotObject;
        this.slotObject.transform.SetParent(this.transform);
        this.slotObject.transform.localScale = Vector3.one;
        this.slotObject.transform.localPosition = Vector3.zero;
        this.slotObject.transform.localRotation = Quaternion.identity;
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
}

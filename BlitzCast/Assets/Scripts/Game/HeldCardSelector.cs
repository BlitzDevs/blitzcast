using UnityEngine;

public class HeldCardSelector : CardSlotKeyboardSelector
{

    protected override void Activate(GameObject selectedGameObject)
    {
        if (selectedGameObject.GetComponent<HeldCardSlot>().slotObject != null)
        {
            selectedGameObject.GetComponent<HeldCardSlot>()
                    .slotObject.GetComponent<CardManager>().TryCastCard();
        }
    }

}

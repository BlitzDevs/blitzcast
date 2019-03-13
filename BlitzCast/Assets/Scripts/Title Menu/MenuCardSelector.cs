using UnityEngine;

public class MenuCardSelector : CardSlotKeyboardSelector
{

    protected override void Activate(GameObject selectedGameObject)
    {
        selectedGameObject.GetComponent<MenuCardSlot>().Activate();
    }


}

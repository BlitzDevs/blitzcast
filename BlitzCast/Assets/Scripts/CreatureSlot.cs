using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSlot : Slot
{
    public Text healthText;
    public Text attackText;
    public Text speedText;
    public Slider attackSlider;
    public GameObject statsDisplay;

    private void Start()
    {
        statsDisplay.SetActive(false);
    }

    public override void SetObject(GameObject slotObject)
    {
        this.slotObject = slotObject;
        this.slotObject.transform.SetParent(this.transform);
        this.slotObject.transform.SetAsFirstSibling();
        this.slotObject.transform.localScale = Vector3.one;
        this.slotObject.transform.localPosition = Vector3.zero;
        this.slotObject.transform.localRotation = Quaternion.identity;
    }

}

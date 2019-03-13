using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardSlotKeyboardSelector : MonoBehaviour
{

    private EventSystem eventSystem;
    public CardSlot currentCardSlot;

    protected abstract void Activate(GameObject selectedGameObject);


    void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }

    void Update()
    {
        currentCardSlot = eventSystem.currentSelectedGameObject != null ?
            eventSystem.currentSelectedGameObject.GetComponent<CardSlot>() : null;

        if (!Mathf.Approximately(0, Mathf.Abs(Input.GetAxis("Horizontal"))) &&
            currentCardSlot == null)
        {
            eventSystem.SetSelectedGameObject(transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (Input.GetKey((i+1).ToString()))
            {
                eventSystem.SetSelectedGameObject(transform.GetChild(i).gameObject);
            }
        }


        if (currentCardSlot != null && Input.GetButtonDown("Submit"))
        {
            Activate(eventSystem.currentSelectedGameObject);
        }

    }


}

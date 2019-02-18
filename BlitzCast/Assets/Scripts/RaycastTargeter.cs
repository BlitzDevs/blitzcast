using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RaycastTargeter : MonoBehaviour
{
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }

    public bool InCastZone()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    public GameObject GetTarget()
    {
        return Raycast()[0].gameObject.GetComponent<CastZone>().GetTargetObject();
    }

    public Transform GetCastingSlot()
    {
        return Raycast()[0].gameObject.GetComponent<CastZone>().GetCastingSlot();
    }

    //TODO: Target specific layer (Cast Zone, Card Targetable, ...)
    private List<RaycastResult> Raycast()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        //foreach (RaycastResult result in results)
        //{
        //    Debug.Log("Hit " + result.gameObject.name);
        //}

        results.RemoveAt(0); // remove first, which is card itself; lazy way

        if (results.Count == 0)
        {
            return null;
        }
        return results;
    }

}
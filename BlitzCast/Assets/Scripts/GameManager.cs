using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public enum Team {
        Friendly, 
        Enemy,
        Neutral
    }

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public int maxHealth;
    public PlayerManager playerA;
    //public PlayerManager playerB;
    public Transform handSlotsAParent;
    //public GameObject handSlotsB;
    public Transform draggingCardParent;
    public GameObject circleTimerPrefab;

    // This function is called by Unity on the first frame that the object is active
    void Start()
    {
        playerA.Initialize(Team.Friendly, maxHealth);
        //playerB.Initialize(Team.B, maxHealth);

        for (int i = 0; i < handSlotsAParent.childCount; i++)
        {
            handSlotsAParent.GetChild(i).GetComponent<HandSlot>()
                .Initialize(playerA);
            //handSlotsB[i].Initialize(playerB);
        }
    }


    public CircleTimer NewTimer(Transform parent)
    {
        CircleTimer timer = null;
        GameObject timerObject = Instantiate(circleTimerPrefab);
        timer = timerObject.GetComponent<CircleTimer>();
        timer.transform.SetParent(parent);
        timer.transform.localPosition = Vector3.zero;
        timer.transform.localScale = Vector3.one;
        timer.gameObject.SetActive(false);
        return timer;
    }

    public List<GameObject> GetAllUnderCursor()
    {
        List<GameObject> results = new List<GameObject>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> rayCast = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, rayCast);

        foreach (RaycastResult r in rayCast)
        {
            results.Add(r.gameObject);
        }
        return results;
    }

    public GameObject GetFirstUnderCursor<T>()
    {
        List<GameObject> hitObjects = GetAllUnderCursor();
        foreach (GameObject g in hitObjects)
        {
            T t = g.GetComponent<T>();
            if (t != null)
            {
                return g;
            }
        }
        return null;
    }

    public GameObject GetFirstUnderCursor(int layer)
    {
        List<GameObject> hitObjects = GetAllUnderCursor();
        foreach (GameObject g in hitObjects)
        {
            if (g.layer == layer)
            {
                return g;
            }
        }
        return null;
    }

}

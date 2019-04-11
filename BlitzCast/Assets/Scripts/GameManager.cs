using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public enum Team {
        Friendly, 
        Enemy,
        Neutral
    }

    public int playerHealth;
    public CreatureGrid creatureGrid;
    public Vector2Int creatureGridSize = new Vector2Int(4, 7);

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public GameTimer timer;
    public PlayerManager playerA;
    //public PlayerManager playerB;
    public Transform handSlotsAParent;
    //public GameObject handSlotsB;
    public Transform dragLocationParent;
    public GameObject circleTimerPrefab;


    // This function is called by Unity on the first frame that the object is active
    void Start()
    {
        // Initialize Players
        playerA.Initialize(Team.Friendly, playerHealth);
        //playerB.Initialize(Team.B, maxHealth);

        // Initialize HandSlots
        for (int i = 0; i < handSlotsAParent.childCount; i++)
        {
            handSlotsAParent.GetChild(i).GetComponent<HandSlot>().Initialize(playerA);
            //handSlotsB[i].Initialize(playerB);
        }

        // Initialize CreatureGrid
        creatureGrid.Initialize(creatureGridSize);
    }


    public CircleTimer NewCircleTimer(Transform parent)
    {
        CircleTimer cTimer = null;
        GameObject timerObject = Instantiate(circleTimerPrefab);
        cTimer = timerObject.GetComponent<CircleTimer>();
        cTimer.transform.SetParent(parent);
        cTimer.transform.localPosition = Vector3.zero;
        cTimer.transform.localScale = Vector3.one;
        cTimer.gameObject.SetActive(false);
        return cTimer;
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

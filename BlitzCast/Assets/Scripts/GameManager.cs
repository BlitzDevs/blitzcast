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

    public int playerHealth = 100;
    public int cardHandSize = 4;
    public Vector2Int creatureGridSize = new Vector2Int(4, 7);
    public CreatureGrid creatureGrid;

    public GameTimer timer;
    public PlayerManager playerA;
    //public PlayerManager playerB;
    public Camera mainCamera;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public Transform dragLocationParent;

    public GameObject circleTimerPrefab;
    public GameObject spellCardPrefab;
    public GameObject creatureCardPrefab;
    public GameObject statusPrefab;


    // This function is called by Unity on the first frame that the object is active
    void Start()
    {
        // Initialize CreatureGrid
        creatureGrid.Initialize(creatureGridSize);
        // Initialize Players
        playerA.Initialize(Team.Friendly, playerHealth, cardHandSize);
        //playerB.Initialize(Team.Enemy, playerHealth, cardHandSize);
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

    public PlayerManager GetPlayer(Team team)
    {
        return playerA;
        //return team == Team.Friendly ? playerA : playerB;
    }

}

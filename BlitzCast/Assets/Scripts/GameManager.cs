using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// The GameManager contains references to important game settings, components
/// and prefabs. It also has utility functions and handles the start and end of
/// the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Team; used to distinguish between players.
    /// </summary>
    public enum Team {
        Friendly, 
        Enemy,
        Neutral
    }

    // Game settings for players
    public int playerHealth = 100;
    public int cardHandSize = 4;

    // References to important components
    public Vector2Int creatureGridSize;
    public CreatureGrid creatureGrid;
    public GameTimer timer;
    public PlayerManager playerA;
    public PlayerManager playerB;
    public Camera mainCamera;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public Transform dragLocationParent;

    // Prefabs
    public GameObject circleTimerPrefab;
    public GameObject handSlotPrefab;
    public GameObject spellCardPrefab;
    public GameObject creatureCardPrefab;
    public GameObject cardDisplayPrefab;
    public GameObject statusPrefab;


    /// <summary>
    /// Called by Unity. First frame this component is active,
    /// initialize CreatureGrid and PlayerManagers.
    /// </summary>
    void Start()
    {
        // Initialize CreatureGrid
        creatureGrid.Initialize(creatureGridSize);
        // Initialize Players
        playerA.Initialize(Team.Friendly, playerHealth, cardHandSize);
        playerB.Initialize(Team.Enemy, playerHealth, cardHandSize);
    }

    /// <summary>
    /// Create a new circle timer.
    /// (I put
    /// </summary>
    public CircleTimer NewCircleTimer(Transform parent)
    {
        CircleTimer cTimer = null;
        GameObject timerObject = Instantiate(circleTimerPrefab, parent);
        cTimer = timerObject.GetComponent<CircleTimer>();
        cTimer.gameObject.SetActive(false);
        return cTimer;
    }

    /// <summary>
    /// Gets all GameObjects under cursor.
    /// </summary>
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

    /// <summary>
    /// Gets the first GameObject under cursor with the specified component.
    /// </summary>
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

    /// <summary>
    /// Gets the first GameObject under cursor in the specified layer.
    /// </summary>
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

    /// <summary>
    /// Gets the player with the corresponding team.
    /// </summary>
    public PlayerManager GetPlayer(Team team)
    {
        return team == Team.Friendly ? playerA : playerB;
    }

}

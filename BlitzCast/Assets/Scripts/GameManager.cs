using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public enum Team {
        A, 
        B,
        Neutral
    }

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public int maxHealth;
    public PlayerManager playerA;
    //public PlayerManager playerB;
    public List<HandSlot> handSlotsA;
    //public GameObject handSlotsB;

    // Use this for initialization
    void Start()
    {
        playerA.Initialize(Team.A, maxHealth);
        //playerB.Initialize(Team.B, maxHealth);

        for (int i = 0; i < handSlotsA.Count; i++)
        {
            handSlotsA[i].Initialize(playerA);
            //handSlotsB[i].Initialize(playerB);
        }
        
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

}

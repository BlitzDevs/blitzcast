using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int coordinates;

    // Use this for initialization
    public void Initialize()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}

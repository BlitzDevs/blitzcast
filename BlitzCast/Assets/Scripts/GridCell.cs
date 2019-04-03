using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int coordinates;
    public Image sprite;
    
    public void HighlightCell(Color color)
    {
        sprite.color = color;        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightCell(Color.yellow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightCell(Color.black);
    }
}

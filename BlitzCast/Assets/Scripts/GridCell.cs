using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int coordinates;
    public Image sprite;
    public bool highlighted;

    public CreatureGrid grid;
    
    public void Highlight(Color color)
    {
        highlighted = true;
        sprite.color = color;
    }

    public void RemoveHighlight()
    {
        highlighted = false;
        sprite.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!highlighted)
        {
            sprite.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!highlighted)
        {
            RemoveHighlight();
        }
    }

    public CreatureCardManager GetCreature()
    {
        return grid.GetCreature(coordinates);
    }
}

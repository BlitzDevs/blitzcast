using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int coordinates;
    public Image sprite;
    public Highlightable highlightable;

    public CreatureGrid grid;

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightable.Highlight(Color.yellow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightable.RemoveHighlight(Color.yellow);
    }

    public CreatureCardManager GetCreature()
    {
        return grid.GetCreature(coordinates);
    }
}

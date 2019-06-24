using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// GridCell defines a single cell on the CreatureGrid. This contains the
/// coordinates (row, column; start on 0).
/// </summary>
public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int coordinates;
    // can highlight this cell
    public Highlightable highlightable;

    public CreatureGrid grid;

    /// <summary>
    /// Called by EventSystem. When hovering this cell, highlight.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightable.Highlight(Color.yellow);
    }

    /// <summary>
    /// Called by EventSystem. When stop hovering this cell, remove highlight.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        highlightable.RemoveHighlight(Color.yellow);
    }

    /// <summary>
    /// Gets creature on itself
    /// Actually uses Grid's GetCreature function
    /// </summary>
    /// <returns>
    /// CreatureCardManager is succeeds, null if fails
    /// </returns>
    public CreatureCardManager GetCreature()
    {
        return grid.GetCreature(coordinates);
    }
}

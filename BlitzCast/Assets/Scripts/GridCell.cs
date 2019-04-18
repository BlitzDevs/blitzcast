using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// GridCell containing some basic information
/// Has its own coordinates in Vector2Int
/// in row column format indexed from 0
/// sprite is blank but lets us recolor sprite
/// Highlightable to let us change colors
/// </summary>
public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int coordinates;
    public Image sprite;
    public Highlightable highlightable;

    public CreatureGrid grid;

    /// <summary>
    /// Cell you're hovering becomes yellow
    /// </summary>
    /// <param name="eventData">
    /// pass in pointer info
    /// </param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightable.Highlight(Color.yellow);
    }

    /// <summary>
    /// so cell doesn't stay yellow
    /// </summary>
    /// <param name="eventData">
    /// pass in pointer info
    /// </param>
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

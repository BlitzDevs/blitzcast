using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Inherit from this to be able to be displayed in DetailViewer.
/// </summary>
public class DetailViewable : Selectable
{
    [SerializeField] protected Highlightable highlightable;
    private Color activeColor = new Color(1f, 1f, 0f, 0.5f);

    private GameManager gameManager;
    private DetailViewer detailViewer;

    private void Start()
    {
        base.Start();

        gameManager = FindObjectOfType<GameManager>();
        detailViewer = gameManager.detailViewer;
    }

    /// <summary>
    /// Called by EventManager; when selected, highlight.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public override void OnSelect(BaseEventData eventData)
    {
        highlightable.Highlight(activeColor);
        detailViewer.Set(gameObject);
    }

    /// <summary>
    /// Called by EventManager; when deselected, remove highlight.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public override void OnDeselect(BaseEventData eventData)
    {
        highlightable.RemoveHighlight(activeColor);
        detailViewer.Set(null);
    }
}

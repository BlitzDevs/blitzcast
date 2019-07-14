using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Changes the color of the image component.
/// Pulse color if multiple highlights.
/// <para>
/// Attach Highlightable onto a GameObject with an Image component.
/// Access this component and use Highlight(color)/RemoveHighlight(color).
/// </para>
/// </summary>
public class Highlightable : MonoBehaviour
{

    // speed to pulse? color pulse not yet implemented
    public float colorPulseSpeed = 1f;

    // reference to image component
    [SerializeField] private List<Image> images;

    // reference to GameTimer, which all time should be based on
    private GameTimer gameTimer;
    // list of current highlights
    private List<Color> highlights;
    // original color of the image
    private List<Color> defaultColors = new List<Color>();
    // index of current color; for color pulsing
    private int colorIndex;


    /// <summary>
    /// Add a highlight to the image.
    /// </summary>
    public void Highlight(Color color)
    {
        highlights.Insert(0, color);
        foreach (Image image in images)
        {
            image.color = color;
        }
        colorIndex = 0;
    }

    /// <summary>
    /// Remove a highlight to the image (should be same color that was added).
    /// </summary>
    public void RemoveHighlight(Color color)
    {
        highlights.Remove(color);
        if (highlights.Count > 0)
        {
            foreach (Image image in images)
            {
                image.color = highlights[0];
            }
        }
        else
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].color = defaultColors[i];
            }
        }
    }

    /// <summary>
    /// Called by Unity. First frame this component is active,
    /// get reference to GameTimer, set image if not exists, and set the image
    /// default color to the color at start.
    /// </summary>
    private void Start()
    {
        gameTimer = FindObjectOfType<GameTimer>();
        highlights = new List<Color>();
        if (images == null)
        {
            images = new List<Image>();
        }
        if (images.Count == 0)
        {
            Image imageOnObject = GetComponent<Image>();
            if (imageOnObject != null)
            {
                images.Add(imageOnObject);
            }
        }
        foreach (Image image in images)
        {
            defaultColors.Add(image.color);
        }
    }

    /// <summary>
    /// Called by Unity. For every frame this component is active,
    /// pulse color depending on pulse speed if there is more than one highlight
    /// color.
    /// </summary>
    private void Update()
    {
        //TODO: Implement

        //// could just be == 0
        //if (highlights.Count > 0 &&
        //    gameTimer.elapsedTime % colorPulseSpeed < 0.001f)
        //{
        //    image.color = highlights[colorIndex];
        //    colorIndex = (colorIndex + 1) % highlights.Count;
        //}
    }

}
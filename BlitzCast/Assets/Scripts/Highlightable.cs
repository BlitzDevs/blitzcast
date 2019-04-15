using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Highlightable : MonoBehaviour
{
    public Image image;
    public List<Color> highlights;
    public float colorPulseSpeed = 1f;

    private Color defaultColor;
    private GameTimer gameTimer;
    private int colorIndex;


    public void Highlight(Color color)
    {
        image.color = color;
        highlights.Insert(0, color);
        colorIndex = 0;
    }

    public void RemoveHighlight(Color color)
    {
        highlights.Remove(color);
        image.color = defaultColor;
    }

    private void Start()
    {
        gameTimer = FindObjectOfType<GameTimer>();
        defaultColor = image.color;
    }

    //private void Update()
    //{
    //    if (highlights.Count > 0 && colorPulseSpeed < gameTimer.elapsedTime)
    //    {
    //        image.color = highlights[colorIndex];
    //        colorIndex = colorIndex >= highlights.Count ? 0 : colorIndex + 1;
    //    }
    //}

}
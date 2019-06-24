using UnityEngine;
using TMPro;

/// <summary>
/// Game timer.
/// </summary>
public class GameTimer : MonoBehaviour
{
    // total elapsed time
    public float elapsedTime;
    // change in time for current frame
    public float deltaTime;

    // reference to displayable text component
    [SerializeField] private TMP_Text text;

    /// <summary>
    /// Called by Unity. For every frame this component is active,
    /// increment the elapsed time and set the timer display text.
    /// </summary>
    void Update()
    {
        deltaTime = Time.deltaTime;
        elapsedTime += deltaTime;
        string displayText = "";

        displayText = string.Format("{0,2}:{1,2}:{2,2}",
            ((int) elapsedTime / 60).ToString().PadLeft(2, '0'), 
            ((int) elapsedTime % 60).ToString().PadLeft(2, '0'),
            (Mathf.RoundToInt(elapsedTime * 100) % 100).ToString().PadLeft(2, '0')
        );
        text.text = displayText;
    }
}

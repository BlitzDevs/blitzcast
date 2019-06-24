using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Performs a countdown and shows the progress visually on a circle.
/// Preferably create using GameManager.
/// </summary>
/// <seealso cref="GameManager.NewCircleTimer(Transform)"/>
public class CircleTimer : MonoBehaviour {

    // original time to countdown from
    public float time;
    // current countdown time
    public float countdown;
    // Entity to base timer speed on, if available
    // e.g. slime speed is 50%, deltaTime is halved
    public Entity entity;
    // color of the circles
    public Color backgroundColor = Color.black;
    public Color fillColor = Color.gray;

    // references to displayable components
    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text text;

    // reference to GameTimer, which all time should be based on
    private GameTimer gameTimer;
    // private variable for calculated time
    private float deltaTime;


    /// <summary>
    /// Called by Unity. First frame this component is active,
    /// get reference to GameTimer and set circle colors.
    /// </summary>
    void Start()
    {
        gameTimer = FindObjectOfType<GameManager>().timer;
        fill.color = fillColor;
        background.color = backgroundColor;
    }

    /// <summary>
    /// Start this timer, counting down from given seconds.
    /// </summary>
    public void StartTimer(float seconds)
    {
        time = seconds;
        countdown = seconds;
    }

    /// <summary>
    /// Called by Unity. Every frame this component is active,
    /// if the timer countdown is incomplete, count down and adjust the display.
    /// </summary>
    void Update () {
        if (!IsComplete())
        {
            // multiply timer speed with entity speed if available
            deltaTime = entity == null ? gameTimer.deltaTime :
                entity.Speed * gameTimer.deltaTime;
            // subtract deltaTime, making sure countdown does not go below 0
            countdown = Mathf.Max(countdown - deltaTime, 0f);
            // set displays
            text.text = Mathf.Round(countdown).ToString();
            fill.fillAmount = countdown / time;
        } 
    }

    /// <summary>
    /// Returns true when the timer is complete (when the countdown is 0).
    /// </summary>
    public bool IsComplete()
    {
        return countdown <= 0;
    }
}

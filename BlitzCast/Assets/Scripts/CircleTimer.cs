using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircleTimer : MonoBehaviour {

    public float time = 3f;
    public float countdown;
    public Entity entity;

    public Color backgroundColor = Color.black;
    public Color fillColor = Color.gray;

    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text text;


    private GameTimer gameTimer;
    private float deltaTime;


    void Start()
    {
        fill.color = fillColor;
        background.color = backgroundColor;
        gameTimer = FindObjectOfType<GameManager>().timer;
    }


    public void StartTimer(float seconds)
    {
        time = seconds;
        countdown = seconds;
    }
	
	void Update () {
        if (!IsComplete())
        {
            deltaTime = entity == null ? gameTimer.deltaTime :
                entity.Speed * gameTimer.deltaTime;
            countdown -= deltaTime;
            text.text = Mathf.Round(countdown).ToString();
            fill.fillAmount = countdown / time;
        } 
    }

    public bool IsComplete()
    {
        return countdown <= 0;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircleTimer : MonoBehaviour {

    public float time = 3f;
    public float countdown;

    public Color backgroundColor = Color.black;
    public Color fillColor = Color.gray;

    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text text;


    public void StartTimer(float seconds)
    {
        time = seconds;
        countdown = seconds;
    }

    void Start () {
        fill.color = fillColor;
        background.color = backgroundColor;
    }
	
	void Update () {
        if (!IsComplete())
        {
            countdown -= Time.deltaTime;
            text.text = Mathf.Round(countdown).ToString();
            fill.fillAmount = countdown / time;
        } 
    }

    public bool IsComplete()
    {
        return countdown <= 0;
    }
}

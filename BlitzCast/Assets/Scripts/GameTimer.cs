using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float elapsedTime;
    public float deltaTime;

    [SerializeField] private TMP_Text text; 


    void Update()
    {
        deltaTime = Time.deltaTime;
        elapsedTime += deltaTime;
        string displayText = "";

        displayText = string.Format("{0,2}:{1,2}:{2,2}",
            ((int)elapsedTime / 60).ToString().PadLeft(2, '0'), 
            ((int)elapsedTime % 60).ToString().PadLeft(2, '0'),
            (Mathf.RoundToInt(elapsedTime * 100) % 100).ToString().PadLeft(2, '0')
        );
        text.text = displayText;
    }
}

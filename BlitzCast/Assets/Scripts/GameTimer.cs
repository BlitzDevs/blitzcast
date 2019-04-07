using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text text; 
    public float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        string displayText = "";
        //don't worry about this :P
        displayText = string.Format("{0,2}:{1,2}:{2,2}",
            ((int)elapsedTime / 60).ToString().PadLeft(2, '0'), 
            ((int)elapsedTime % 60).ToString().PadLeft(2, '0'),
            (Mathf.RoundToInt(elapsedTime * 100) % 100).ToString().PadLeft(2, '0')
        );
        text.text = displayText;
    }
}

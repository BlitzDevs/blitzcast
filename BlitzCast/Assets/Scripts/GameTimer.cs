using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text text; 
    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        text.text = Mathf.Round(elapsedTime).ToString();
    }
}

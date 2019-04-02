using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text text; 
    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        text.text = Mathf.Round(elapsedTime).ToString();
    }
}

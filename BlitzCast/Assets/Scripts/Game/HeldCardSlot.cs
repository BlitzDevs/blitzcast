using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeldCardSlot : CardSlot
{
    public GameObject redrawDisplay;
    public Slider drawTimeSlider;

    private PlayerManager player;

    protected override void Initialize()
    {
        base.Initialize();
        redrawDisplay.SetActive(false);
        player = FindObjectOfType<GameManager>().GetPlayer(team);
    }

    public IEnumerator DrawCard(int drawTimeout)
    {
        redrawDisplay.SetActive(true);

        float timer = 0;
        while (timer < drawTimeout)
        {
            timer += Time.deltaTime;
            drawTimeSlider.value = timer / drawTimeout;

            yield return null;
        }

        redrawDisplay.SetActive(false);
        player.Draw(index);
        
    }

}

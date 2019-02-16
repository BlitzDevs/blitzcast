using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Player player;
    public Player opponent;
    public Text timerText;

    private bool gameEnd = false;

	// Use this for initialization
	void Start ()
    {
        player.Initialize();
        opponent.Initialize();
        StartCoroutine(Timer(Time.fixedTime));
    }
	
	// Update is called once per frame
	void Update () {
       
	}


    public IEnumerator Timer(float startTime)
    {
        while (!gameEnd)
        {
            timerText.text = Math.Round(Time.fixedTime - startTime, 1).ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public void Cast(Card card)
    {
        if (card.behaviorValues.Count != card.behaviors.Count)
            Debug.LogWarning("CardBehaviors not parallel to BehaviorValues");

        for (int i = 0; i < card.behaviors.Count; i++)
        {
            switch (card.behaviors[i])
            {
                case Card.CardBehavior.DamageTarget:
                    {
                        Debug.Log("Damage target " + card.behaviorValues[i]);
                        break;
                    }
                case Card.CardBehavior.Counter:
                    {
                        Debug.Log("Counter card");
                        break;
                    }
                default:
                    {
                        Debug.Log("CardBehavior " + card.behaviors[i].ToString() + " has not been implemented.");
                        break;
                    }
            }
        }
    }
}

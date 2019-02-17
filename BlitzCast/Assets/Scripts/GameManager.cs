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
	void Update ()
    {
       
	}


    public IEnumerator Timer(float startTime)
    {
        while (!gameEnd)
        {
            timerText.text = Math.Round(Time.fixedTime - startTime, 1)
                .ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public void Cast(Card card, GameObject target)
    {
        Card cardTarget;

        if (card.behaviorValues.Count != card.behaviors.Count)
            Debug.LogWarning("CardBehaviors not parallel to BehaviorValues");

        for (int i = 0; i < card.behaviors.Count; i++)
        {
            int value = card.behaviorValues[i];
            switch (card.behaviors[i])
            {
                case Card.CardBehavior.DamageTarget:
                    {
                        Debug.Log("Try damage target " + value);

                        if (IsValidTargetType(target, typeof(IEntity)))
                        {
                            Debug.Log("Is Entity");
                            IEntity targetEntity = target.GetComponent<IEntity>();
                            targetEntity.Damage(value);
                        }
                        else
                        {
                            Debug.Log("Not valid target");
                        }

                    }
                    break;
                case Card.CardBehavior.Counter:
                    {
                        Debug.Log("Try counter card");

                        if (IsValidTargetType(target, typeof(Card)))
                        {
                            Card targetCard = target.GetComponent<CardManager>().card;
                            Debug.Log("Counter card " + targetCard.name);
                        }
                        else
                        {
                            Debug.Log("Not valid target");
                        }

                    }
                    break;
                default:
                    {
                        Debug.Log("CardBehavior " + card.behaviors[i].ToString() + " has not been implemented.");
                        break;
                    }
            }
        }
    }

    public bool IsValidTargetType(GameObject target, Type type)
    {
        if (type == typeof(Card) &&
            target.GetComponent<CardManager>() != null &&
            target.GetComponent<CardManager>().card != null)
        {
            return target.GetComponent<CardManager>().card.GetType() == type;
        }
        else if (type == typeof(IEntity))
        {
            // is Card with IEntity OR is Player
            return (target.GetComponent<CardManager>() != null &&
                target.GetComponent<CardManager>().card is IEntity) ||
                target.GetComponent<Player>() != null;
        }

        return false;
    }

}

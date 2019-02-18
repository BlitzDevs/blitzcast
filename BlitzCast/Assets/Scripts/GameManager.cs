using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum Team
    {
        A,
        B,
        Neutral
    }

    public Player playerA;
    public Player playerB;
    public GameObject spellCardPrefab;
    public GameObject creatureCardPrefab;
    public RaycastTargeter targeter;
    public Text timerText;

    public Team userTeam; // client

    private bool gameEnd = false;

	// Use this for initialization
	void Start ()
    {
        targeter = FindObjectOfType<RaycastTargeter>();

        playerA.Initialize(Team.A);
        playerB.Initialize(Team.B);

        userTeam = Team.A; // temporary

    StartCoroutine(Timer(Time.fixedTime));
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

    public void Cast(GameObject cardObject, GameObject target)
    {
        Card card = cardObject.GetComponent<CardManager>().card;
        if (card.behaviorValues.Count != card.behaviors.Count)
            Debug.LogWarning("CardBehaviors not parallel to BehaviorValues");

        for (int i = 0; i < card.behaviors.Count; i++)
        {
            int value = card.behaviorValues[i];
            switch (card.behaviors[i])
            {
                case Card.CardBehavior.Creature:
                    {
                        // add checks to ensure is creaturecard or move entirely
                        CreatureManager creatureManager = cardObject
                            .GetComponent<CreatureManager>();


                        // PLACEHOLDER instantiate creature object


                        StartCoroutine(creatureManager.ActivateCreature());
                    }
                    break;

                case Card.CardBehavior.DamageTarget:
                    {
                        Debug.Log("Try damage target " + value);

                        if (IsValidTargetType(target, typeof(IEntity)))
                        {
                            IEntity targetEntity = target
                                .GetComponent<IEntity>();
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
                            Card targetCard = target
                                .GetComponent<CardManager>().card;
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
                        Debug.Log("CardBehavior " + card.behaviors[i].ToString()
                            + " has not been implemented.");
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

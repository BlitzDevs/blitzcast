using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

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
    public Targeter targeter;
    public Text timerText;

    public Team userTeam; // client

    private bool gameEnd = false;

	// Use this for initialization
	void Start ()
    {
        targeter = FindObjectOfType<Targeter>();

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

    public IEnumerator CastCard(GameObject cardObject, CastZone castZone)
    {
        CardManager cardManager = cardObject.GetComponent<CardManager>();
        Card card = cardManager.card;
        GameObject target = castZone.GetTargetObject();
        Transform slot = castZone.GetCastingSlot();

        if (slot != null)
        {
            cardObject.transform.SetParent(slot);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }


        card.SetStatus(Card.CardStatus.Casting);
        cardManager.CastingAnimation();

        for (var i = card.castTime; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            cardManager.SetCastTimer(i);
        }

        card.Cast(target);

        card.SetStatus(Card.CardStatus.Recharging);
        cardManager.RechargingAnimation();

        for (var i = card.redrawTime; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);
            cardManager.SetRedrawTimer(i);
        }

        card.SetStatus(Card.CardStatus.Deck);
        card.GetOwner().Draw();
        //Destroy(cardObject);
    }

}

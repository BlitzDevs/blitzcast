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

    public PlayerManager playerA;
    public PlayerManager playerB;
    public GameObject spellCardPrefab;
    public GameObject creatureCardPrefab;
    public Targeter targeter;
    public Text timerText;

    public Team userTeam; // client

    private bool gameEnd = false;
    private int handSize = 4;
    private int maxHealth = 50;

	// Use this for initialization
	void Start ()
    {
        targeter = FindObjectOfType<Targeter>();

        // Initialize is necessary due to race condition--
        // Player needs to know its team before it starts, which means
        // GameManager has to set it first
        playerA.Initialize(Team.A, handSize, maxHealth);
        playerB.Initialize(Team.B, handSize, maxHealth);

        userTeam = Team.A; // temporary

        StartCoroutine(Timer(Time.fixedTime));
    }

    public PlayerManager GetPlayer(Team team)
    {
        switch (team)
        {
            case Team.A: return playerA;
            case Team.B: return playerB;
            default: return null;
        }
    }


    public IEnumerator Timer(float startTime)
    {
        while (!gameEnd)
        {

            float time = Time.time - startTime;
            int min = (int)Math.Floor(time / 60);
            int sec = (int)Math.Floor(time % 60);
            int ms = (int)Math.Floor((time - sec) * 100);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, ms);
            yield return null;
        }
    }


    public IEnumerator CastCard(CardSlot cardSlot, CastZone castZone)
    {
        GameObject cardObject = cardSlot.slotObject;
        CardManager cardManager = cardObject.GetComponent<CardManager>();
        Card card = cardManager.card;
        GameObject target = castZone.GetTargetObject();
        Transform slot = castZone.GetCastingSlot();

        StartCoroutine(cardSlot.DrawCard(card.redrawTime));

        if (slot != null)
        {
            cardObject.transform.SetParent(slot);
            cardObject.transform.localScale = Vector3.one;
            cardObject.transform.localPosition = Vector3.zero;
            cardObject.transform.localRotation = Quaternion.identity;
        }
        cardObject.transform.localPosition = Vector3.zero;


        card.SetStatus(Card.CardStatus.Casting);
        cardManager.CastingAnimation();

        float timer = 0;
        while (timer < card.castTime)
        {
            timer += Time.deltaTime;
            cardManager.SetCastTimer(timer);
            yield return null;
        }

        card.Cast(cardObject, target);
    }

    public IEntity GetCreatureAttackTarget(int index, Team team)
    {
        Team targetTeam = team == Team.A ? Team.B : (team == Team.B ? Team.A : Team.Neutral);
        PlayerManager targetPlayer = GetPlayer(targetTeam);
        return targetPlayer.creatureSlots[index].slotObject != null ?
            (IEntity) targetPlayer.creatureSlots[index].slotObject.GetComponent<CreatureManager>() :
            (IEntity) targetPlayer;
    }

}

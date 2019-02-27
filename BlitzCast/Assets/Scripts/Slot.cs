using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Slot : CastZone
{

    protected Card card;
    public int index;
    public GameObject slotObject;

    protected GameManager.Team team;
    protected GameManager.Team userTeam;


    void Start()
    {
        userTeam = FindObjectOfType<GameManager>().userTeam;
    }


    public void SetCard(GameObject cardObject)
    {
        this.card = cardObject.GetComponent<CardManager>().card;
        this.slotObject = cardObject;
        this.slotObject.transform.SetParent(this.transform);
        this.slotObject.transform.localScale = Vector3.one;
        this.slotObject.transform.localPosition = Vector3.zero;
        this.slotObject.transform.localRotation = Quaternion.identity;
    }

    public void SetTeam(GameManager.Team team)
    {
        this.team = team;
    }

    public void RemoveCard()
    {
        this.card = null;
    }

}

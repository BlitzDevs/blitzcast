using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour,
                        IPointerEnterHandler, IPointerExitHandler {

    public int index;
    public GameObject cardObject;

    private GameManager.Team team;
    private GameManager.Team userTeam;

    private Card card;
    private Vector2 originalPosition = new Vector2();
    private const int pixelsToFloatWhenSelected = 20;


    void Start()
    {
        userTeam = FindObjectOfType<GameManager>().userTeam;
    }


    // TODO: (Future) Selectable with keys
    // TODO: Fix ordering, not float when dragging
    public void OnPointerEnter(PointerEventData eventData)
    {
        Float();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unfloat();
    }

    public void Float()
    {
        if (team == userTeam && card.status == Card.CardStatus.Held)
        {
            originalPosition = cardObject.transform.localPosition;
            cardObject.transform.localPosition = new Vector3(
                originalPosition.x,
                originalPosition.y + pixelsToFloatWhenSelected,
                0);
        }
    }

    public void Unfloat()
    {
        if (team == userTeam && card.status == Card.CardStatus.Held)
        {
            cardObject.transform.localPosition = originalPosition;
        }
    }

    public void SetCard(GameObject cardObject)
    {
        this.cardObject = cardObject;
        this.card = cardObject.GetComponent<CardManager>().card;
        this.cardObject.transform.SetParent(this.transform);
        this.cardObject.transform.localScale = Vector3.one;
        this.cardObject.transform.localPosition = Vector3.zero;
        this.cardObject.transform.localRotation = Quaternion.identity;
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

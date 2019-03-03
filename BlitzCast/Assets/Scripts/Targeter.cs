using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Targeter : MonoBehaviour
{
    public enum Tag
    {
        CastZone,
        CreatureSlot,
        CastingCard,
        Player,
        Creature,
        CardSlot
    }

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private Dictionary<Card.CardAction, List<Tag>> actionTargets =
        new Dictionary<Card.CardAction, List<Tag>>
        {
            {
                Card.CardAction.Creature, new List<Tag>(new Tag[]
                {
                    Tag.CreatureSlot
                })
            },
            {
                Card.CardAction.DamageTarget, new List<Tag>(new Tag[]
                {
                    Tag.Player, Tag.Creature
                })
            },
            {
                Card.CardAction.DamageAll, new List<Tag>(new Tag[]
                {
                    Tag.CastZone
                })
            },
            {
                Card.CardAction.BuffAttack, new List<Tag>(new Tag[]
                {
                    Tag.Creature
                })
            },
            {
                Card.CardAction.Counter, new List<Tag>(new Tag[]
                {
                    Tag.CastingCard
                })
            }
        };


    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }

    public CastZone GetTargetCastZone(Card.CardAction action, GameObject ignoreObject)
    {
        List<GameObject> hitObjects = new List<GameObject>();
        List<RaycastResult> results = Raycast();

        foreach (RaycastResult result in results)
        {
            if (result.gameObject != ignoreObject)
            {
                //Debug.Log("Hit " + result.gameObject.name);
                hitObjects.Add(result.gameObject);
            }
        }

        List<Tag> tags = new List<Tag>();
        actionTargets.TryGetValue(action, out tags);

        foreach (GameObject hitObject in hitObjects)
        {
            CastZone hitCastZone = hitObject.GetComponent<CastZone>();
            GameObject hitTarget = hitCastZone != null ? hitCastZone.GetTargetObject() : null;

            if (hitCastZone != null && hitCastZone.GetTargetObject() != null)
            {
                foreach (Tag targetTag in tags)
                {
                    if ((targetTag == Tag.CastZone)
                        || (targetTag == Tag.CreatureSlot && hitTarget.GetComponent<CreatureSlot>() != null)
                        || (targetTag == Tag.CastingCard && hitTarget.GetComponent<CardManager>().card.StatusIs(Card.CardStatus.Casting))
                        || (targetTag == Tag.Player && hitTarget.GetComponent<PlayerManager>() != null)
                        || (targetTag == Tag.Creature && hitTarget.GetComponent<CreatureSlot>() != null && hitTarget.GetComponent<CreatureSlot>().slotObject != null)
                        || (targetTag == Tag.CardSlot && hitTarget.GetComponent<CardSlot>() != null))
                    {
                        Debug.Log("Target: " + hitObject.name);
                        return hitObject.GetComponent<CastZone>();
                    }
                }
            }
        }

        return null;
    }



    private List<RaycastResult> Raycast()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        return results;
    }


}
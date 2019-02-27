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
        List<RaycastResult> results = Raycast();
        List<GameObject> hitObjects = new List<GameObject>();
        GameObject targetObject = null;

        foreach (RaycastResult result in results)
        {
            if (result.gameObject != ignoreObject)
            {
                Debug.Log("Hit " + result.gameObject.name);
                hitObjects.Add(result.gameObject);
            }
        }

        //CastZone,
        //CreatureSlot,
        //CastingCard,
        //Player,
        //Creature,
        //CardSlot

        List<Tag> tags = new List<Tag>();
        actionTargets.TryGetValue(action, out tags);

        foreach (GameObject hitObject in hitObjects)
        {
            CastZone hitCastZone = hitObject.GetComponent<CastZone>();

            foreach (Tag targetTag in tags)
            {
                switch (targetTag)
                {
                    case Tag.CastZone:
                        if (hitCastZone != null)
                        {
                            targetObject = hitObject;
                        }
                        break;
                    case Tag.CreatureSlot:
                        if (hitCastZone != null && hitCastZone.GetTargetObject()
                            .GetComponent<CreatureSlot>() != null)
                        {
                            targetObject = hitObject;
                        }
                        break;
                    case Tag.CastingCard:
                        if (hitCastZone != null && hitCastZone.GetTargetObject()
                            .GetComponent<CardManager>()
                            .card.StatusIs(Card.CardStatus.Casting))
                        {
                            targetObject = hitObject;
                        }
                        break;
                    case Tag.Player:
                        Debug.Log("Target tag is Player");
                        if (hitCastZone != null && hitCastZone.GetTargetObject()
                            .GetComponent<Player>() != null)
                        {
                            targetObject = hitObject;
                        }
                        break;
                }
            }
        }

        if (targetObject != null)
        {
            Debug.Log("Target: " + targetObject.name);
            return targetObject.GetComponent<CastZone>();
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
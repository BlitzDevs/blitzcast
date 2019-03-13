using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Targeter : MonoBehaviour
{

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;


    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }

    public CastZone GetTargetCastZone(List<Card.Behavior> behaviors, GameObject ignoreObject) {
        List<Card.Target> targets = new List<Card.Target>();
        foreach (Card.Behavior behavior in behaviors)
        {
            foreach (Card.Target target in targets)
            {
                targets.Add(target);
            }
        }

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

        //List<Func<GameObject, bool>> lambdas = new List<Func<GameObject, bool>>();
        //foreach (Card.Target t in targets)
        //{
        //    Func<GameObject, bool> tlambda;
        //    targetLambdas.TryGetValue(t, out tlambda);
        //    if (tlambda != null)
        //    {
        //        lambdas.Add(tlambda);
        //    }
        //}

        bool found = false;
        foreach (GameObject hitObject in hitObjects)
        {
            CastZone hitCastZone = hitObject.GetComponent<CastZone>();
            GameObject hitTarget = hitCastZone != null ? hitCastZone.GetTargetObject() : null;

            if (hitCastZone != null && hitCastZone.GetTargetObject() != null)
            {
                foreach (Card.Target target in targets)
                {
                    switch (target)
                    {
                        case Card.Target.CastZone:
                            {
                                found = hitTarget.GetComponent<CastZone>() != null;
                                break;
                            }
                        case Card.Target.HeldCardSlot:
                            {
                                found = hitTarget.GetComponent<HeldCardSlot>() != null;
                                break;
                            }
                        case Card.Target.CreatureSlot:
                            {
                                found = hitTarget.GetComponent<CreatureSlot>() != null && hitTarget.GetComponent<CreatureSlot>().slotObject == null;
                                break;
                            }
                        case Card.Target.Creature:
                            {
                                found = hitTarget.GetComponent<CreatureSlot>() != null && hitTarget.GetComponent<CreatureSlot>().slotObject != null;
                                break;
                            }
                        case Card.Target.CastingCard:
                            {
                                found = hitTarget.GetComponent<CardManager>().GetCard().StatusIs(Card.Status.Casting);
                                break;
                            }
                        case Card.Target.Opponent:
                            {
                                found = hitTarget.GetComponent<PlayerManager>() != null && !hitTarget.GetComponent<PlayerManager>().IsUser();
                                break;
                            }
                        case Card.Target.User:
                            {
                                found = hitTarget.GetComponent<PlayerManager>() != null && hitTarget.GetComponent<PlayerManager>().IsUser();
                                break;
                            }
                        default:
                            {
                                Debug.LogWarning("Unimplemented target: " + target.ToString());
                                break;
                            }
                    }

                    if (found)
                    {
                        Debug.Log("Target: " + hitObject.name);
                        return hitObject.GetComponent<CastZone>();
                    }
                }

                //foreach (Card.CardTarget targetCard.CardTarget in targets)
                //{
                //    if ((targetCard.CardTarget == Card.CardTarget.CastZone)
                //        || (targetCard.CardTarget == Card.CardTarget.CreatureSlot && hitTarget.GetComponent<CreatureSlot>() != null)
                //        || (targetCard.CardTarget == Card.CardTarget.CastingCard && hitTarget.GetComponent<CardManager>().GetCard().StatusIs(Card.CardStatus.Casting))
                //        || (targetCard.CardTarget == Card.CardTarget.Player && hitTarget.GetComponent<PlayerManager>() != null)
                //        || (targetCard.CardTarget == Card.CardTarget.Creature && hitTarget.GetComponent<CreatureSlot>() != null && hitTarget.GetComponent<CreatureSlot>().slotObject != null)
                //        || (targetCard.CardTarget == Card.CardTarget.CardSlot && hitTarget.GetComponent<HeldCardSlot>() != null))
                //    {
                //        Debug.Log("Target: " + hitObject.name);
                //        return hitObject.GetComponent<CastZone>();
                //    }
                //}
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
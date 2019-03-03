using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Card", menuName = "Spell Card")]
public class SpellCard : Card
{

    public override Card Clone()
    {
        SpellCard copy = (SpellCard) CreateInstance(typeof(SpellCard));

        copy.cardName = this.cardName;
        copy.description = this.description;
        copy.art = this.art;
        copy.castTime = this.castTime;
        copy.redrawTime = this.redrawTime;
        copy.behavior = this.behavior;

        return copy;
    }

    public override void Cast(GameObject selfObject, GameObject target)
    {
        SetStatus(CardStatus.Active);
        switch (behavior.action)
        {
            case CardAction.DamageTarget:
                {
                    IEntity targetEntity = target.GetComponent<CreatureSlot>() != null ?
                        target.GetComponent<CreatureSlot>().slotObject.GetComponent<CreatureManager>() : target.GetComponent<IEntity>();

                    targetEntity.Damage(behavior.value);
                }
                break;

            case CardAction.Counter:
                {
                    Card targetCard = target.GetComponent<CardManager>().card;
                    Debug.Log("Counter card " + targetCard.cardName);
                }
                break;

            default:
                {
                    Debug.Log("CardBehavior " + behavior.ToString()
                        + " has not been implemented.");
                }
                break;
        }
        SetStatus(CardStatus.Deck);
        Destroy(selfObject);
    }


}

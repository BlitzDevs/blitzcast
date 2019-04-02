using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellCardManager: CardManager
{
    [SerializeField] protected Text descriptionText;

    new void Start()
    {
        base.Start();

        descriptionText.text = card.description;
    }

    public override void Cast(List<GameObject> Targets) {
        SpellCard spellCard = (SpellCard) card;
        foreach (Card.Behavior behavior in spellCard.behaviors)
        {
            switch (behavior.action)
            {
                case Card.Action.Damage:
                    break;
                case Card.Action.Destroy:
                    break;
                case Card.Action.GiveStatus:
                    break;
                case Card.Action.Heal:
                    break;
                default:
                    break;
            }

        }

    }
}

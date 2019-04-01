using UnityEngine;
using System.Collections.Generic;

public class SpellCardManager: CardManager
{
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

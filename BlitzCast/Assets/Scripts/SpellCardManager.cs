using UnityEngine;
using TMPro;

public class SpellCardManager: CardManager
{
    [SerializeField] protected TMP_Text descriptionText;


    public override void Initialize(Card card, GameManager.Team team, HandSlot slot)
    {
        this.card = card;
        this.team = team;
        this.slot = slot;

        descriptionText.text = card.description;
    }

    public override void Cast(GameObject target) {
        SpellCard spellCard = (SpellCard) card;
        switch (spellCard.cardBehavior.action)
        {
            case Card.Action.Damage:
                break;
            case Card.Action.Destroy:
                break;
            case Card.Action.Heal:
                break;
            default:
                break;
        }

    }

    public override void EnablePreview(GameObject target)
    {
        SpellCard spellCard = (SpellCard) card;

    }
    public override void DisablePreview()
    {
        Debug.Log("Disable Preview");
    }

    public override GameObject GetCastTarget()
    {
        SpellCard spellCard = (SpellCard) card;
        if (spellCard.cardBehavior.targetArea == Card.TargetArea.Single)
        {
            switch (spellCard.cardBehavior.action)
            {
                case Card.Action.Damage:
                    break;
                case Card.Action.Destroy:
                    break;
                case Card.Action.Heal:
                    break;
            }
        } else
        {
            // When target area is a shape (not single), target is Creature Grid
            return gameManager.GetFirstUnderCursor<GridCell>();
        }

        return null;
    }


}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Mini displayer for held cards.
/// </summary>
public class CardDisplayer : MonoBehaviour
{

    // FOR REFERENCE:
    // TMP_Text is TextMeshPro text; much better than default Unity text

    public Transform spriteMaskParent;
    // References to display components
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text raceText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject timeCostsParent;
    [SerializeField] private TMP_Text castTimeText;
    [SerializeField] private TMP_Text drawTimeText;
    [SerializeField] private GameObject actionParent;
    [SerializeField] private Image actionIcon;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private TMP_Text actionValueText;
    [SerializeField] private GameObject statusParent;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text stacksText;
    [SerializeField] private GameObject statParent;
    [SerializeField] private Image statIcon;
    [SerializeField] private TMP_Text statModifierValueText;
    [SerializeField] private GameObject conditionParent;
    [SerializeField] private Image conditionIcon;
    [SerializeField] private TMP_Text conditionText;
    [SerializeField] private GameObject creatureStatsParent;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text speedText;

    /// <summary>
    /// Set this CardDisplayer; show values according to card.
    /// </summary>
    public void Set(Card card)
    {
        // always have time costs
        timeCostsParent.SetActive(true);
        castTimeText.text = card.castTime.ToString();
        drawTimeText.text = card.redrawTime.ToString();

        if (nameText != null)
        {
            nameText.text = card.cardName;
        }
        if (raceText != null)
        {
            raceText.text = card.race.GetLongName();
        }
        // if card has action, show
        if (card.action != Card.Action.None)
        {
            actionParent.SetActive(true);
            actionText.text = card.action.GetShortName();
            actionValueText.text = card.actionValue.ToString();
            string fp = "Textures/Icons/" + card.action.ToString().ToLower();
            Sprite actionSprite = Resources.Load<Sprite>(fp + "-icon");
            actionIcon.sprite = actionSprite;
        }
        else
        {
            actionParent.SetActive(false);
        }
        // if card has status inflict, show
        if (card.statusInflicted != Card.Status.None)
        {
            statusParent.SetActive(true);
            statusText.color = card.statusInflicted.GetDisplayColor();
            statusText.text = card.statusInflicted.GetShortName();
            stacksText.text = card.stacks.ToString();
        }
        else
        {
            statusParent.SetActive(false);
        }
        // if card has stat change inflict, show
        if (card.statChange != Card.StatChange.None)
        {
            statParent.SetActive(true);
            statModifierValueText.text = card.statChangeValue.ToString();
            string fp = "Textures/Icons/" + card.statChange.ToString().ToLower();
            Sprite statSprite = Resources.Load<Sprite>(fp + "-icon");
            statIcon.sprite = statSprite;
            statIcon.color = card.statChange.GetDisplayColor();
        }
        else
        {
            statParent.SetActive(false);
        }
        // if card has cast condition, show
        if (card.condition != Card.Condition.None)
        {
            conditionParent.SetActive(true);
            // TODO: show condition value depending on condition
            conditionText.text = card.condition.GetShortName();
            string fp = "Textures/Icons/" + card.condition.ToString().ToLower();
            Sprite conditionSprite = Resources.Load<Sprite>(fp + "-icon");
            conditionIcon.sprite = conditionSprite;
            conditionIcon.color = card.condition.GetDisplayColor();
        }
        else
        {
            conditionParent.SetActive(false);
        }
        // if card is Creature, show health/speed
        if (card is CreatureCard creatureCard)
        {
            creatureStatsParent.SetActive(true);
            healthText.text = creatureCard.health.ToString();
            speedText.text = creatureCard.actionTime.ToString();
        }
        else
        {
            creatureStatsParent.SetActive(false);
        }
    }
}

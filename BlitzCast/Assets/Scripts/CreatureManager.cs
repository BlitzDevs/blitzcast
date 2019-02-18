using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour {

    public Text healthText;
    public Text attackText;
    public Text speedText;
    public CardManager cardManager;

    private CreatureCard creatureCard; // stats x=health, y=attack, z=speed

    void Start()
    {
        creatureCard = (CreatureCard) cardManager.card;
        healthText.text = creatureCard.GetStats().x.ToString();
        attackText.text = creatureCard.GetStats().y.ToString();
        speedText.text = creatureCard.GetStats().z.ToString();
    }

    public IEnumerator ActivateCreature()
    {
        while (creatureCard.GetStats().x > 0) // health > 0
        {
            yield return new WaitForSecondsRealtime(creatureCard.GetStats().y);
            Debug.Log(creatureCard.name + " attack placeholder");
        }

        // health < 0
        Destroy(this.gameObject);

    }
}

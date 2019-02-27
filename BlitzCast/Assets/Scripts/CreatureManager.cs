using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour
{

    public Text healthText;
    public Text attackText;
    public Text speedText;
    public GameObject sprite;
    public Animator animator;
    public CardManager cardManager;

    private CreatureCard creatureCard; // stats x=health, y=attack, z=speed

    void Start()
    {
        creatureCard = (CreatureCard) cardManager.card;
        creatureCard.SetCreatureManager(this);
        SetStatsTexts(creatureCard.GetStats());
    }

    public void SetStatsTexts(Vector3Int stats)
    {
        healthText.text = stats.x.ToString();
        attackText.text = stats.y.ToString();
        speedText.text = stats.z.ToString();
    }

    public void CardIntoSprite()
    {
        gameObject.name = "Creature";
        RectTransform rt = (RectTransform)gameObject.transform;
        rt.sizeDelta = new Vector2(64f, 64f);

        sprite.SetActive(true);
        Debug.Log(sprite.activeInHierarchy);
        Debug.Log(creatureCard.animator.name);
        animator.runtimeAnimatorController = creatureCard.animator;
        cardManager.enabled = false;
    }

    public void DestroySelf()
    {
        Debug.Log(creatureCard.cardName + " died.");
        Destroy(this.gameObject);
    }

}

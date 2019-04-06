using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HandSlot : Selectable, IDeselectHandler, ISelectHandler,
                            IPointerEnterHandler, IPointerExitHandler
{

    public GameObject slotObject;
    [SerializeField] private GameObject drawTimerObject;
    [SerializeField] private Image drawTimer;
    [SerializeField] private TMP_Text drawTimerText;

    private Vector2 originalPosition = Vector2.zero;
    private const int pixelsToFloatWhenSelected = 20;
    private EventSystem eventSystem;
    private PlayerManager player;


    // Called by GameManager
    public void Initialize(PlayerManager player)
    {
        eventSystem = FindObjectOfType<EventSystem>();
        this.player = player;
        drawTimerObject.SetActive(false);
        DrawCard();
    }

    public void StartCardDrawTimer(float time)
    {
        StartCoroutine(CardDrawTimer(time));
    }

    private IEnumerator CardDrawTimer(float time)
    {
        drawTimerObject.SetActive(true);

        float countdown = time;
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            drawTimerText.text = Mathf.Round(countdown).ToString();
            yield return null;
        }

        drawTimerObject.SetActive(false);
        DrawCard();
    }

    private void DrawCard()
    {
        Card card = player.DrawTop();
        GameObject cardPrefab = null;
        if (card is CreatureCard)
        {
            cardPrefab = player.creatureCardPrefab;
        } else if (card is SpellCard)
        {
            cardPrefab = player.spellCardPrefab;
        } else
        {
            Debug.LogError("Card is not CreatureCard or SpellCard >:(");
        }

        GameObject cardObject = Instantiate(cardPrefab);
        cardObject.GetComponent<CardManager>().Initialize(card, player.team, this);

        slotObject = cardObject;
        slotObject.transform.SetParent(transform);
        slotObject.transform.localScale = Vector3.one;
        slotObject.transform.localPosition = Vector3.zero;
        slotObject.transform.localRotation = Quaternion.identity;
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (slotObject != null)
        {
            // Float
            slotObject.transform.localPosition = new Vector3(
                originalPosition.x, originalPosition.y + pixelsToFloatWhenSelected, 0);
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (slotObject != null)
        {
            // Unfloat
            slotObject.transform.localPosition = originalPosition;
        }
    }

}

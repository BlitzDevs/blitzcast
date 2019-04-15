using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Card card;
    public GameManager.Team team;
    public SmoothMover cardMover;
    public SmoothMover spriteMover;

    // SeralizeField tells the Unity editor to display variable even if private
    // protected is a private variable accessible by inheriting members
    [SerializeField] protected GameObject cardFront;
    [SerializeField] protected GameObject cardBack;
    [SerializeField] protected GameObject targetableZone;
    [SerializeField] protected SpriteSheetAnimator animator;
    [SerializeField] protected RectTransform castingSpriteParent;
    [SerializeField] protected Image sprite;
    [SerializeField] protected Image artImage;
    // TMP_Text is TextMeshPro text; much better than default Unity text
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text raceText;
    [SerializeField] protected TMP_Text redrawTimeText;
    [SerializeField] protected TMP_Text castTimeText;


    protected GameManager gameManager;
    protected CreatureGrid grid;
    protected HandSlot slot;
    protected List<Highlightable> previewHighlightables;

    private PlayerManager player;
    private CircleTimer castTimer;
    private bool casted = false;


    // abstract functions are to be implemented by inherting classes
    // HashSets are Collections which can only contain unique values
    abstract public GameObject GetCastLocation();
    abstract public List<GameObject> GetCastTargets(GameObject target);
    abstract public HashSet<GameObject> GetActionTargets(GameObject location);
    abstract public void TryPreview();
    abstract public void Cast(GameObject location);


    // virtual functions are overrideable but can have a body
    // Initialize is called by HandSlot
    public virtual void Initialize(Card card, HandSlot slot, PlayerManager player)
    {
        gameManager = FindObjectOfType<GameManager>();
        grid = gameManager.creatureGrid;

        previewHighlightables = new List<Highlightable>();
        castTimer = gameManager.NewCircleTimer(transform);
        castTimer.gameObject.SetActive(false);

        this.card = card;
        this.slot = slot;
        this.player = player;
        team = player.team;

        gameObject.layer = LayerMask.NameToLayer("Held");

        nameText.text = card.cardName;
        raceText.text = card.race.ToString();
        artImage.color = card.color;
        sprite.color = card.color;
        SpriteSheetAnimator.Animatable anim = new SpriteSheetAnimator.Animatable(
            card.name,
            "Cards/" + (card is CreatureCard ? "Creatures" : "Spells"),
            card.spriteAnimateSpeed,
            null
        );
        animator.Initialize(anim);
        castTimeText.text = card.castTime.ToString();
        redrawTimeText.text = card.redrawTime.ToString();

        castingSpriteParent.gameObject.SetActive(false);
    }

    public virtual void DestroySelf()
    {
        ClearPreview();
        Destroy(castingSpriteParent.gameObject);
        Destroy(gameObject);
    }

    protected void ClearPreview()
    {
        foreach (Highlightable h in previewHighlightables)
        {
            h.RemoveHighlight(card.color);
        }
        previewHighlightables.Clear();
    }

    // When begin dragging card, move card to Active layer
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (casted)
        {
            return;
        }

        if (gameObject.layer == LayerMask.NameToLayer("Held")) {
            // change layer to Active
            gameObject.layer = LayerMask.NameToLayer("Active");
            // highlight card to show selected
            SetTint(new Color(1f, 1f, 0f, 0.5f));
            // enable sprite for preview
            castingSpriteParent.gameObject.SetActive(true);
            // move sprite to dragging in hierarchy
            castingSpriteParent.SetParent(gameManager.dragLocationParent);
        }
    }

    // While dragging, move the card and try preview
    public void OnDrag(PointerEventData eventData)
    {
        if (casted)
        {
            return;
        }

        // Convert screen location to camera position
        Vector3 pointPosition = new Vector3(
            eventData.position.x,
            eventData.position.y,
            gameManager.mainCamera.nearClipPlane
        );
        //castingSpriteParent.transform.position = gameManager.mainCamera.ScreenToWorldPoint(pointPosition);
        spriteMover.SetPosition(gameManager.mainCamera.ScreenToWorldPoint(pointPosition));

        ClearPreview();
        TryPreview();

    }

    // When stop dragging, start casting if valid; else return to hand
    public void OnEndDrag(PointerEventData eventData)
    {
        if (casted)
        {
            return;
        }

        GameObject target = GetCastLocation();
        if (target != null)
        {
            casted = true;

            //Finish the move before disabling the movers
            cardMover.InstantMove();
            spriteMover.InstantMove();

            //disable card and sprite movers so they can't move after casting
            //might need to change for creature attack animations
            cardMover.enabled = false;
            spriteMover.enabled = false;

            // remove self from card slot
            slot.slotObject = null;
            // highlight card blue to show casting
            SetTint(new Color (0.2f, 0.2f, 1f, 0.5f));

            // tell player that card casted this frame
            player.entity.TriggerActionEvent();
            
            StartCoroutine(CastTimer(target));
        }
        else
        {
            // change layer to Held
            gameObject.layer = LayerMask.NameToLayer("Held");
            // remove card highlight
            SetTint(new Color(0, 0, 0, 0));
            // move card back to slot
            transform.SetParent(slot.transform);
            transform.position = slot.transform.position;
            // return sprite location and disable
            castingSpriteParent.transform.SetParent(transform);
            castingSpriteParent.transform.localPosition = Vector3.zero;
            castingSpriteParent.gameObject.SetActive(false);
        }

    }

    protected virtual IEnumerator CastTimer(GameObject target)
    {
        castTimer.gameObject.SetActive(true);
        castTimer.StartTimer(card.castTime);
        while (!castTimer.IsComplete())
        {
            yield return null;
        }
        castTimer.gameObject.SetActive(false);

        // start card draw timer after casted
        slot.StartDrawTimer(card.redrawTime);
        // cast
        Cast(target);

        ClearPreview();
    }

    protected HashSet<GameObject> FilterTargetsByCondition(HashSet<GameObject> targets)
    {
        // Filter targets based on conditions
        HashSet<GameObject> targetsCopy = new HashSet<GameObject>(targets);

        foreach (GameObject t in targetsCopy)
        {
            bool valid = true;

            Entity tEntity = t.GetComponent<Entity>();
            CardManager tCard = t.GetComponent<CardManager>();

            switch (card.condition)
            {
                case Card.Condition.None:
                    break;

                case Card.Condition.HPGreaterThan:
                    if (tEntity != null)
                    {
                        valid = tEntity.Health > card.conditionValue;
                    }
                    break;

                case Card.Condition.HPLessThan:
                    if (tEntity != null)
                    {
                        valid = tEntity.Health < card.conditionValue;
                    }
                    break;

                case Card.Condition.Race:

                    if (tCard != null)
                    {
                        valid = tCard.card.race ==
                            (Card.Race) card.conditionValue;
                    }
                    break;

                case Card.Condition.Friendly:
                    if (tCard != null)
                    {
                        valid = tCard.team == GameManager.Team.Friendly;
                    }
                    break;

                case Card.Condition.Enemy:
                    if (tCard != null)
                    {
                        valid = tCard.team == GameManager.Team.Enemy;
                    }
                    break;

                case Card.Condition.Status:
                    valid = false;
                    foreach (Entity.Status s in tEntity.statuses)
                    {
                        if ((int)s.statusType == card.conditionValue)
                        {
                            valid = true;
                        }
                    }
                    break;

                default:
                    Debug.LogWarning("Condition not implemented");
                    break;
            }

            // Remove target if marked invalid
            if (!valid)
            {
                targets.Remove(t);
            }
        }

        return targets;
    }

    protected void ExecuteActionOnTargets(HashSet<GameObject> targets)
    {
        // Execute Card Action on our list of valid targets
        foreach (GameObject t in targets)
        {
            Entity tEntity = t.GetComponent<Entity>();
            CardManager tCard = t.GetComponent<CardManager>();

            switch (card.action)
            {
                case Card.Action.None:
                    break;

                case Card.Action.Damage:
                    if (tEntity != null)
                        tEntity.Health -= card.actionValue;
                    break;

                case Card.Action.Heal:
                    tEntity.Health += card.actionValue;
                    break;

                case Card.Action.Destroy:
                    tCard.DestroySelf();
                    break;

                default:
                    Debug.LogWarning("Card Action not implemented");
                    break;
            }

            switch (card.statChange)
            {
                case Card.StatChange.None:
                    break;

                case Card.StatChange.SetHealth:
                    tEntity.Health = card.statChangeValue;
                    tEntity.MaxHealth = card.statChangeValue;
                    break;

                case Card.StatChange.IncreaseHealth:
                    tEntity.MaxHealth += card.statChangeValue;
                    break;

                case Card.StatChange.IncreaseSpeed:
                    tEntity.Speed += card.statChangeValue / 100f;
                    break;

                default:
                    Debug.LogWarning("Card Stat Change not implemented");
                    break;
            }

            tEntity.ApplyStatus(
                card.statusInflicted,
                card.stacks
            );
        }
    }

    // in the future, replace this with some animation possibly
    public void SetTint(Color color)
    {
        targetableZone.GetComponent<Image>().color = color;
    }

}

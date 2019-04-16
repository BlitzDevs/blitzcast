using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// The abstract CardManager that other CardManagers should inherit from.
/// Handles all of the displays and events that a card object does.
/// The card itself is determined by Card.
/// </summary>
/// <seealso cref="Card"/>
/// <seealso cref="CreatureCardManager"/>
/// <seealso cref="SpellCardManager"/>
public abstract class CardManager : MonoBehaviour,
                           IBeginDragHandler, IDragHandler, IEndDragHandler
{

    // FOR REFERENCE:
    // [SeralizeField]  tells the Unity editor to display variable even if private
    // protected        private variable accessible by inheriting members
    // abstract         functions to be implemented by inherting classes
    // virtual          functions that are overrideable but can have a body
    // HashSet          Collections which can only contain unique values


    public Card card;
    public GameManager.Team team;
    // smooth movements for moving our card (the peeking) and sprite (casting)
    public SmoothMover cardMover;
    public SmoothMover spriteMover;

    // These fields are references to our displayable components.
    // These are set through the Unity Editor and should already be set in the
    // prefab of the card.
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

    // references to useful objects
    protected GameManager gameManager;
    protected CreatureGrid grid;
    protected HandSlot slot;

    // list of highlightable components we are currently highlighting for preview
    // this is needed so that we can remove our highlights after done
    protected List<Highlightable> previewHighlightables;

    private PlayerManager player;
    private CircleTimer castTimer;

    // once we know we have casted, we want to be able to prevent some actions
    private bool casted = false;

    /// <summary>
    /// Get the GameObject of a valid cast location based on where the
    /// mouse cursor currently is.
    /// </summary>
    /// <returns>
    /// The GameObject of a valid cast location; otherwise, null.
    /// </returns>
    abstract public GameObject GetCastLocation();

    /// <summary>
    /// Get the targets for the cast based on the location of the cast.
    /// </summary>
    /// <returns>
    /// List of GameObjects containing the target GameObjects.
    /// </returns>
    abstract public List<GameObject> GetCastTargets(GameObject locationObject);

    /// <summary>
    /// Get the targets for the action based on the location GameObject and the
    /// action type of this card.
    /// </summary>
    /// <returns>
    /// List of GameObjects containing the target GameObjects.
    /// </returns>
    abstract public HashSet<GameObject> GetActionTargets(GameObject locationObject);

    /// <summary>
    /// Called every frame inside OnDrag(); for showing cast location through
    /// cell highlighting.
    /// </summary>
    abstract public void TryPreview();

    /// <summary>
    /// Cast this card onto a location GameObject. Only a valid locationObject
    /// should be passed.
    /// </summary>
    abstract public void Cast(GameObject locationObject);

    /// <summary>
    /// Initialize this CardManager; set values and initiate displays.
    /// (This function should be called by HandSlot after drawing/creating
    /// the card.)
    /// </summary>
    /// <param name="card">
    /// The card which defines the values and behavior of this card object.
    /// </param>
    /// <param name="slot">
    /// The card hand slot which this card belongs to. DrawCard() will be
    /// called to this hand slot.
    /// </param>
    /// <param name="player">
    /// The reference to the player, which is needed to determine team.
    /// </param>
    public virtual void Initialize(Card card, HandSlot slot, PlayerManager player)
    {
        // find our GameManager so that we can have a reference to it
        gameManager = FindObjectOfType<GameManager>();
        grid = gameManager.creatureGrid;

        // initiate highlightables list
        previewHighlightables = new List<Highlightable>();

        // create castTimer object and deactivate it for now
        castTimer = gameManager.NewCircleTimer(transform);
        castTimer.gameObject.SetActive(false);

        // set our variables
        this.card = card;
        this.slot = slot;
        this.player = player;
        team = player.team;

        // on creation, this card should be Held
        gameObject.layer = LayerMask.NameToLayer("Held");

        // set the display texts/colors to their proper values
        nameText.text = card.cardName;
        raceText.text = card.race.ToString();
        artImage.color = card.color;
        sprite.color = card.color;
        animator.Initialize(
            card.name,
            "Cards/" + (card is CreatureCard ? "Creatures" : "Spells"),
            card.spriteAnimateSpeed,
            null
        );
        castTimeText.text = card.castTime.ToString();
        redrawTimeText.text = card.redrawTime.ToString();

        castingSpriteParent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Do some clean up, then destroy this gameObject.
    /// </summary>
    public virtual void DestroySelf()
    {
        ClearPreview();
        Destroy(castingSpriteParent.gameObject);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called every frame within OnDrag.
    /// Removes all preview highlights.
    /// </summary>
    protected void ClearPreview()
    {
        foreach (Highlightable h in previewHighlightables)
        {
            h.RemoveHighlight(card.color);
        }
        previewHighlightables.Clear();
    }

    /// <summary>
    /// Unity Event. On first frame of dragging this object, change the layer
    /// and displays.
    /// Does not do anything if card has already been casted.
    /// </summary>
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

    /// <summary>
    /// Unity Event. While dragging this object, set the position of the sprite
    /// to the mouse and show previews of the cast if valid.
    /// Does not do anything if card has already been casted.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (casted)
        {
            return;
        }

        // convert screen location to camera position
        // this is necessary because the Canvas's Render Mode is Camera
        Vector3 pointPosition = new Vector3(
            eventData.position.x,
            eventData.position.y,
            gameManager.mainCamera.nearClipPlane
        );
        spriteMover.SetPosition(gameManager.mainCamera.ScreenToWorldPoint(pointPosition));

        // update the cell highlighting/preview as we move mouse
        ClearPreview();
        TryPreview();
    }

    /// <summary>
    /// Unity Event. Once finished dragging this object, depending on whether
    /// the location of the current mouse position points to a GameObject which
    /// is a valid cast location, begin the cast and prepare displays. Else,
    /// reset the position of the sprite back into the hand slot.
    /// Does not do anything if card has already been casted.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (casted)
        {
            return;
        }

        GameObject target = GetCastLocation();
        if (target != null)
        {
            // set our casted flag so that cannot cast twice/move this object
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

            // begin the cast timer
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

    /// <summary>
    /// Start timer, then cast this card with the passed target once the timer
    /// is complete.
    /// <para>
    /// Use with StartCoroutine(CastTimer).
    /// </para>
    /// </summary>
    /// <param name="target">
    /// The target location of the cast, as determined by GetCastLocation()
    /// previously.
    /// </param>
    protected virtual IEnumerator CastTimer(GameObject target)
    {
        // start the cast timer
        castTimer.gameObject.SetActive(true);
        castTimer.StartTimer(card.castTime);
        while (!castTimer.IsComplete())
        {
            // stuck here until timer is complete
            yield return null;
        }
        castTimer.gameObject.SetActive(false);

        // start card draw timer after casted
        slot.StartDrawTimer(card.redrawTime);

        ClearPreview();
        // cast
        Cast(target);
    }

    /// <summary>
    /// Filter a list of GameObject targets depending on the condition type and
    /// value of this card.
    /// </summary>
    protected HashSet<GameObject> FilterTargetsByCondition(HashSet<GameObject> targets)
    {
        // A copy of our list is needed because you cannot modify a list that
        // you are looping over.
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
                        valid = tCard.card.race == (Card.Race) card.conditionValue;
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

            // Remove target (from original list) if marked invalid
            if (!valid)
            {
                targets.Remove(t);
            }
        }

        return targets;
    }

    /// <summary>
    /// Unity Event. Once finished dragging this object, depending on whether
    /// the location of the current mouse position points to a GameObject which
    /// is a valid cast location, begin the cast and prepare displays. Else,
    /// reset the position of the sprite back into the hand slot.
    /// Does not do anything if card has already been casted.
    /// </summary>
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

            tEntity.ApplyStatus(new Entity.StatusModifier(
                card.statusInflicted,
                card.stacks
            ));
        }
    }


    /// <summary>
    /// Temporary display function which changes the color of the targetableZone
    /// which overlays on top of the card. Cheap, but useful.
    /// TODO: Replace this with an animation, possibly.
    /// </summary>
    public void SetTint(Color color)
    {
        targetableZone.GetComponent<Image>().color = color;
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerManager : MonoBehaviour, IEntity, IHighlightable
{
    public Player player;
    public GameManager.Team team;
    public int health;
    public int maxHealth;
    public List<Status> statuses;
    public bool highlighted;

    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image frame;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject handSlotPrefab;
    [SerializeField] private Transform handSlotParent;
    [SerializeField] private Transform statusesParent;
    [SerializeField] private SpriteSheetAnimator animator;

    [SerializeField] private List<Card> playingDeck; // deck in play

    private GameManager gameManager;
    private System.Random randomGenerator = new System.Random();
    private int frameDamage;
    private bool castedThisFrame;
    private float deltaTimeForCardDraw;


    public void Initialize(GameManager.Team team, int maxHealth, int handSize)
    {
        gameManager = FindObjectOfType<GameManager>();
        statuses = new List<Status>();
        this.team = team;

        // initialize Player
        this.maxHealth = maxHealth;
        SetHealth(maxHealth);
        SpriteSheetAnimator.Animatable anim = new SpriteSheetAnimator.Animatable(
            player.caster.name,
            "Casters",
            player.caster.spriteAnimateSpeed
        );
        animator.Initialize(anim);
        usernameText.text = player.username;

        CloneDeck();
        Shuffle();

        // create and initialize HandSlots
        for (int i = 0; i < handSize; i++)
        {
            GameObject handSlotObject = Instantiate(handSlotPrefab, handSlotParent);
            HandSlot handSlot = handSlotObject.GetComponent<HandSlot>();
            handSlot.Initialize(this);
        }
    }

    public Card DrawTop()
    {
        Card temp = playingDeck[0];
        playingDeck.RemoveAt(0);

        if (playingDeck.Count == 0)
        {
            CloneDeck();
            Shuffle();
        }

        return temp;
    }

    public void CloneDeck()
    {
        playingDeck = new List<Card>(player.deck.Count);

        foreach (Card c in player.deck)
        {
            playingDeck.Add(c);
        }
    }

    public void Shuffle()
    {
        for (int i = 0; i < playingDeck.Count; i++)
        {
            int j = randomGenerator.Next(0, playingDeck.Count);
            var temp = playingDeck[i];
            playingDeck[i] = playingDeck[j];
            playingDeck[j] = temp;
        }
    }

    public void SetHealthText(int hp)
    {
        healthText.text = hp.ToString();
        healthSlider.value = (float) hp / maxHealth;
    }

    public void Damage(int hp)
    {
        SetHealth(health -= hp);
    }

    public void Heal(int hp)
    {
        SetHealth(Math.Min(health + hp, maxHealth));
    }

    public void IncreaseHP(int hp)
    {
        maxHealth += hp;
        SetHealth(health + hp);
    }

    public void SetHealth(int hp)
    {
        health = hp;
        // change health display
        SetHealthText(health);
    }

    public int GetHealth()
    {
        return health;
    }

    void Update()
    {
        deltaTimeForCardDraw = gameManager.timer.deltaTime;
        DoStatuses();
        frameDamage = 0;
        castedThisFrame = false;
    }

    public void DoStatuses()
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            // Careful! Structs are immutable types in C#,
            // so have to make a new Status when changing a value.
            Status status = statuses[i];

            switch (status.statusType)
            {
                case Card.StatusType.Stun:
                    // stop timer from progressing
                    deltaTimeForCardDraw -= gameManager.timer.deltaTime;
                    // after 1 second, remove 1 stack
                    if (gameManager.timer.elapsedTime - statuses[i].startTime > 1f)
                    {
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            gameManager.timer.elapsedTime
                        );
                    }
                    break;

                case Card.StatusType.Poison:
                    // after 1 second, deal (stacks) damage and remove 1 stack
                    if (gameManager.timer.elapsedTime - statuses[i].startTime > 1f)
                    {
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            gameManager.timer.elapsedTime
                        );
                        frameDamage = status.stacks;
                    }
                    break;

                case Card.StatusType.Shield:
                    if (frameDamage > 0)
                    {
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            status.startTime
                        );
                        frameDamage = 0;
                    }
                    break;

                case Card.StatusType.Wound:
                    if (castedThisFrame)
                    {
                        //we don't care about startTime for wounded
                        statuses[i] = new Status(
                            status.statusType,
                            status.stacks - 1,
                            status.startTime
                        );
                        frameDamage = status.stacks;
                    }
                    break;
                default:
                    Debug.LogWarning("Status not implemented: " + status.statusType.ToString());
                    break;
            }

            if (status.stacks == 0)
            {
                statuses.RemoveAt(i);
                i--;
            }
        }
    }

    public void ApplyStatus(Card.StatusType statusType, int stacks)
    {
        // if status already exists, add stacks
        for (int i = 0; i < statuses.Count; i++)
        {
            Status s = statuses[i];
            if (s.statusType == statusType)
            {
                s.stacks += stacks;
                return;
            }
        }

        // otherwise add
        statuses.Add(new Status(statusType, stacks, gameManager.timer.elapsedTime));
        Color color;
        switch (statusType)
        {
            case Card.StatusType.Stun:
                color = Color.yellow;
                break;
            case Card.StatusType.Poison:
                color = Color.magenta;
                break;
            case Card.StatusType.Wound:
                color = Color.red;
                break;
            case Card.StatusType.Clumsy:
                color = Color.green;
                break;
            case Card.StatusType.Shield:
                color = Color.blue;
                break;
            default:
                color = Color.black;
                break;
        }

        GameObject statusObject = Instantiate(gameManager.statusPrefab, statusesParent);
        Image statusImage = statusObject.GetComponent<Image>();
        statusImage.color = color;
    }

    public List<Status> GetStatuses()
    {
        return statuses;
    }

    public float GetDeltaTime()
    {
        return deltaTimeForCardDraw;
    }

    public void CastedThisFrame()
    {
        castedThisFrame = true;
    }

    public void Highlight(Color color)
    {
        highlighted = true;
        frame.color = color;
    }

    public void RemoveHighlight()
    {
        highlighted = false;
        frame.color = Color.white;
    }
}


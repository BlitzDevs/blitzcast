using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
    public struct Status
    {
        public Card.StatusType statusType;
        public int stacks;
        public float startTime;

        public Status(Card.StatusType statusType, int stacks, float startTime)
        {
            this.statusType = statusType;
            this.stacks = stacks;
            this.startTime = startTime;
        }
    }

    public delegate void ActionHandler();
    public event ActionHandler ActionEvent;
    public delegate void DeathHandler(Entity sender);
    public event DeathHandler DeathEvent;
    public delegate void IntChangeHandler(int i);
    public event IntChangeHandler HealthChangeEvent;
    public event IntChangeHandler MaxHealthChangeEvent;
    public delegate void FloatChangeHandler(float i);
    public event FloatChangeHandler SpeedChangeEvent;

    public List<Status> statuses = new List<Status>();

    private Transform statusesParent;

    private GameManager gameManager;

    private int _health;
    private int _maxHealth;
    private float _speed;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = Mathf.Min(value, _maxHealth);
            OnHealthChange(_health);
        }
    }

    public float Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
            OnSpeedChange(value);
        }
    }


    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            Health += value - _maxHealth;
            _maxHealth = value;
            OnMaxHealthChange(value);
        }
    }

    private void OnHealthChange(int hp)
    {
        if (HealthChangeEvent != null)
        {
            HealthChangeEvent(hp);
        }
    }

    private void OnSpeedChange(float s)
    {
        if (SpeedChangeEvent != null)
        {
            SpeedChangeEvent(s);
        }
    }

    private void OnMaxHealthChange(int hp)
    {
        if (MaxHealthChangeEvent != null)
        {
            MaxHealthChangeEvent(hp);
        }
    }


    public void Initialize(int health, float speed, List<Status> statuses,
        Transform statusesParent)
    {
        gameManager = FindObjectOfType<GameManager>();

        MaxHealth = health;
        Health = health;
        Speed = speed;
        this.statuses = statuses;

        this.statusesParent = statusesParent;
    }

    public void OnDoAction() {
        //handle Wounded
    }
    
    public void ApplyStatus(Card.StatusType statusType, int stacks)
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            // Careful! Structs are immutable types in C#,
            // so have to make a new Status when changing a value.
        //    Status status = statuses[i];

        //    switch (status.statusType)
        //    {
        //        case Card.StatusType.Stun:
        //            // stop timer from progressing
        //            deltaTimeForCardDraw -= gameManager.timer.deltaTime;
        //            // after 1 second, remove 1 stack
        //            if (gameManager.timer.elapsedTime - statuses[i].startTime > 1f)
        //            {
        //                statuses[i] = new Status(
        //                    status.statusType,
        //                    status.stacks - 1,
        //                    gameManager.timer.elapsedTime
        //                );
        //            }
        //            break;

        //        case Card.StatusType.Poison:
        //            // after 1 second, deal (stacks) damage and remove 1 stack
        //            if (gameManager.timer.elapsedTime - statuses[i].startTime > 1f)
        //            {
        //                statuses[i] = new Status(
        //                    status.statusType,
        //                    status.stacks - 1,
        //                    gameManager.timer.elapsedTime
        //                );
        //                frameDamage = status.stacks;
        //            }
        //            break;

        //        case Card.StatusType.Shield:
        //            if (frameDamage > 0)
        //            {
        //                statuses[i] = new Status(
        //                    status.statusType,
        //                    status.stacks - 1,
        //                    status.startTime
        //                );
        //                frameDamage = 0;
        //            }
        //            break;

        //        case Card.StatusType.Wound:
        //            if (castedThisFrame)
        //            {
        //                //we don't care about startTime for wounded
        //                statuses[i] = new Status(
        //                    status.statusType,
        //                    status.stacks - 1,
        //                    status.startTime
        //                );
        //                frameDamage = status.stacks;
        //            }
        //            break;
        //        default:
        //            Debug.LogWarning("Status not implemented: " + status.statusType.ToString());
        //            break;
        //    }

        //    if (status.stacks == 0)
        //    {
        //        statuses.RemoveAt(i);
        //        i--;
        //    }
        }


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

        // TODO:
        GameObject statusObject = Instantiate(gameManager.statusPrefab, statusesParent);
        Image statusImage = statusObject.GetComponent<Image>();
        statusImage.color = color;
    }
}

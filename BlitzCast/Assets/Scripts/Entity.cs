using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{

    public class Status
    {
        public enum StatusType
        {
            None,
            Clumsy,
            Wound,
            Stun,
            Poison,
            Shield
        }

        public StatusType statusType;
        public Color color;
        public float startTime;

        private Entity entity;
        private int _stacks;

        public int Stacks
        {
            get
            {
                return _stacks;
            }
            set
            {
                int oldStacks = _stacks;
                _stacks = value;
                entity.OnStackChange(statusType, oldStacks, _stacks);
            }
        }

        public Status(StatusType statusType, Color color, int stacks, float startTime, Entity entity)
        {
            this.statusType = statusType;
            this.color = color;
            this.startTime = startTime;
            this.entity = entity;
            Stacks = stacks;
        }
    }


    public List<Status> statuses;

    public delegate void ActionHandler();
    public event ActionHandler ActionEvent;
    public delegate void IntChangeHandler(int oldInt, int newInt);
    public event IntChangeHandler HealthChangeEvent;
    public event IntChangeHandler MaxHealthChangeEvent;
    public delegate void SpeedChangeHandler(float s);
    public event SpeedChangeHandler SpeedChangeEvent;
    public delegate void StatusChangeHandler(Status.StatusType statusType, int oldStacks, int newStacks);
    public event StatusChangeHandler StatusChangeEvent;

    private GameManager gameManager;
    private Transform statusesParent;
    private Dictionary<Status.StatusType, TMP_Text> statusDisplays;

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
            int oldHealth = _health;
            _health = Mathf.Min(value, _maxHealth);
            OnHealthChange(oldHealth, _health);
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
            int oldMaxHealth = _maxHealth;
            _maxHealth = value;
            Health += _maxHealth - oldMaxHealth;
            OnMaxHealthChange(oldMaxHealth, _maxHealth);
        }
    }

    private void OnHealthChange(int oldHP, int newHP)
    {
        if (HealthChangeEvent != null)
        {
            HealthChangeEvent(oldHP, newHP);
        }
    }

    private void OnSpeedChange(float s)
    {
        if (SpeedChangeEvent != null)
        {
            SpeedChangeEvent(s);
        }
    }

    private void OnMaxHealthChange(int oldHP, int newHP)
    {
        if (MaxHealthChangeEvent != null)
        {
            MaxHealthChangeEvent(oldHP, newHP);
        }
    }

    private void OnStackChange(Status.StatusType statusType, int oldStacks, int newStacks)
    {
        if (StatusChangeEvent != null)
        {
            StatusChangeEvent(statusType, oldStacks, newStacks);
        }
    }


    public void Initialize(int health, float speed, Transform statusesParent)
    {
        gameManager = FindObjectOfType<GameManager>();

        MaxHealth = health;
        Health = health;
        Speed = speed;

        statusDisplays = new Dictionary<Status.StatusType, TMP_Text>();
        statuses = new List<Status>
        {
            new Status(Status.StatusType.Clumsy, Color.green, 0, 0f, this),
            new Status(Status.StatusType.Wound, Color.red, 0, 0f, this),
            new Status(Status.StatusType.Stun, Color.yellow, 0, 0f, this),
            new Status(Status.StatusType.Poison, Color.magenta, 0, 0f, this),
            new Status(Status.StatusType.Shield, Color.blue, 0, 0f, this)
        };
        this.statusesParent = statusesParent;

        StatusChangeEvent += SetStatusDisplay;
    }

    public void TriggerActionEvent()
    {
        if (ActionEvent != null)
        {
            ActionEvent();
        }
    }

    public void ApplyStatus(Status.StatusType statusType, int stacks)
    {
        if (statusType == Status.StatusType.None || stacks == 0)
        {
            return;
        }

        Status status = GetStatus(statusType);
        status.Stacks += stacks;

        if (status.Stacks - stacks == 0)
        {
            switch (statusType)
            {
                case Status.StatusType.Clumsy:
                    // different for creature and player...
                    break;

                case Status.StatusType.Wound:
                    ActionEvent += Wound;
                    break;

                case Status.StatusType.Stun:
                    StartCoroutine(Stun());
                    break;

                case Status.StatusType.Poison:
                    StartCoroutine(Poison());
                    break;

                case Status.StatusType.Shield:
                    HealthChangeEvent += Shield;
                    break;

                default:
                    Debug.LogWarning("Status not implemented");
                    break;
            }
        }

    }

    private void Wound()
    {
        Status status = GetStatus(Status.StatusType.Wound);
        Health -= status.Stacks;
        status.Stacks--;

        if (status.Stacks < 1)
        {
            ActionEvent -= Wound;
        }
    }

    private IEnumerator Stun()
    {
        Status status = GetStatus(Status.StatusType.Stun);
        status.startTime = gameManager.timer.elapsedTime;
        Speed = 0;

        while (status.Stacks > 0)
        {
            if (gameManager.timer.elapsedTime - status.startTime > 1f)
            {
                status.Stacks--;
                status.startTime = gameManager.timer.elapsedTime;
            }
            yield return null;
        }

        Speed = 1f;
    }

    private IEnumerator Poison()
    {
        Status status = GetStatus(Status.StatusType.Poison);

        while (status.Stacks > 0)
        {
            if (gameManager.timer.elapsedTime - status.startTime > 1f)
            {
                Health -= status.Stacks;
                status.Stacks--;
                status.startTime = gameManager.timer.elapsedTime;
            }
            yield return null;
        }
    }

    private void Shield(int oldHP, int newHP)
    {
        Status status = GetStatus(Status.StatusType.Shield);

        // if damaged, negate
        if (newHP - oldHP < 0)
        {
            Health = oldHP;
            status.Stacks--;

            if (status.Stacks < 1)
            {
                HealthChangeEvent -= Shield;
            }
        }
    }

    public Status GetStatus(Status.StatusType type)
    {
        int i = (int)type - 1;
        if (i < 0 || i > statuses.Count)
        {
            return null;
        }
        return statuses[(int) type - 1];
    }

    // added onto event StatusChangeEvent
    public void SetStatusDisplay(Status.StatusType statusType, int oldStacks, int newStacks)
    {
        Status status = GetStatus(statusType);
        TMP_Text statusDisplay;
        statusDisplays.TryGetValue(statusType, out statusDisplay);

        if (oldStacks < 1 && newStacks > 0)
        {
            GameObject statusObject = Instantiate(gameManager.statusPrefab, statusesParent);
            Image statusImage = statusObject.GetComponent<Image>();
            statusImage.color = status.color;
            statusDisplay = statusObject.GetComponentInChildren<TMP_Text>();
            statusDisplays.Add(statusType, statusDisplay);
        }
        else if (oldStacks > 0 && newStacks < 1)
        {
            statusDisplays.Remove(statusType);
            Destroy(statusDisplay.transform.parent.gameObject);
        }

        if (statusDisplay != null)
        {
            statusDisplay.text = newStacks.ToString();
        }
    }
}
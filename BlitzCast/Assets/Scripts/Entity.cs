using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Entity : MonoBehaviour
{

    // FOR REFERENCE:
    // [Serializable]  part of System; indicates that object can be serialized,
    //                 or turned into a stream of bytes
    //                 For our purposes: allows Unity to show struct in editor.
    //
    // get/set         All properties have an underlying get/set, but it can be
    //                 manually defined as shown.
    //                 To have a system where we can recieve triggers on
    //                 variable change, inside the set {}, we also call a
    //                 function, which triggers the event.
    //                 An internal variable (convetion is to use _name) is
    //                 needed as reference to the actual value.
    //
    // delegate        defines a reference type that can encapsulate a method
    //                 with the same return value and parameters
    //
    // event           Methods can be added (+=) onto events. When the event is
    //                 triggered, any methods previously added onto it is
    //                 called. The attached methods must follow the event's
    //                 respective delegate. The event must be triggered within
    //                 the class that contains it using the parentheses
    //                 operator; e.g. MyEvent(...)


    /// <summary>
    /// A simple struct containing the StatusType and number of stacks.
    /// Useful for modifying the status of an Entity.
    /// </summary>
    [Serializable] public struct StatusModifier
    {
        public Status.StatusType type;
        [Range(0, 10)] public int stacks;

        public StatusModifier(Status.StatusType type, int stacks)
        {
            this.type = type;
            this.stacks = stacks;
        }
    }

    /// <summary>
    /// A simple struct containing the StatChange and amount.
    /// Useful for modifying a stat of an Entity.
    /// </summary>
    [Serializable] public struct StatModifier
    {
        public Card.StatChange type;
        [Range(-100, 100)] public int value;

        public StatModifier(Card.StatChange type, int value)
        {
            this.type = type;
            this.value = value;
        }
    }

    /// <summary>
    /// A subclass within Entity which defines a Status and holds the properties
    /// needed to execute the status.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Possible status types that can be applied on an Entity.
        /// </summary>
        public enum StatusType
        {
            None,
            Clumsy,
            Wound,
            Stun,
            Poison,
            Shield
        }

        // StatusType defines the behavior
        public StatusType statusType;
        // the color associated with the StatusType
        public Color color;
        // startTime is used for time based statues; e.g. Stun
        public float startTime;

        // reference to Entity, so that we can notify/trigger event if this
        // status changes.
        private Entity entity;

        // the internal stacks amount
        private int _stacks;
        // publicly accessible Stacks value which notifies on change
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
                // tell the entity that this status's stacks has been modified
                entity.OnStackChange(statusType, oldStacks, _stacks);
            }
        }

        // simple constructor for a new Status
        public Status(StatusType statusType, Color color, int stacks, float startTime, Entity entity)
        {
            this.statusType = statusType;
            this.color = color;
            this.startTime = startTime;
            this.entity = entity;
            Stacks = stacks;
        }
    }


    // define delgates to be used for events
    // see reference at top for delegate explanation
    public delegate void ActionHandler();
    public delegate void IntChangeHandler(int oldInt, int newInt);
    public delegate void FloatChangeHandler(float oldFloat, float newFloat);
    public delegate void StatusChangeHandler(Status.StatusType statusType, int oldStacks, int newStacks);

    // define the events we want to be able to be notified of
    public event ActionHandler ActionEvent;
    public event IntChangeHandler HealthChangeEvent;
    public event IntChangeHandler MaxHealthChangeEvent;
    public event FloatChangeHandler SpeedChangeEvent;
    public event StatusChangeHandler StatusChangeEvent;

    // references to useful objects
    private GameManager gameManager;
    // statusesParent is where we can attach status displays to
    private Transform statusesParent;
    // dictionary for accessing the text component based on status type
    private Dictionary<Status.StatusType, TMP_Text> statusDisplays;

    // Entity variables
    public List<Status> statuses;

    // internal variables
    private int _health;
    private int _maxHealth;
    private float _speed;

    /// <summary>
    /// The Health property for Entity.
    /// Will trigger any HealthChange events if they exist.
    /// </summary>
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

    /// <summary>
    /// The Speed property for Entity.
    /// Will trigger any SpeedChange events if they exist.
    /// </summary>
    public float Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            float oldSpeed = _speed;
            _speed = value;
            OnSpeedChange(oldSpeed, _speed);
        }
    }
    
    /// <summary>
    /// The MaxHealth property for Entity.
    /// Will trigger any MaxHealthChange events if they exist.
    /// </summary>
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


    /// <summary>
    /// Initialize this Entity; set values and initiate displays.
    /// (This function should be called right after instantiation.)
    /// </summary>
    /// <param name="health">
    /// The starting Health/MaxHealth.
    /// </param>
    /// <param name="statusesParent">
    /// The parent object in which status icons should be attached to.
    /// </param>
    public void Initialize(int health, Transform statusesParent)
    {
        // find our GameManager so that we can have a reference to it
        gameManager = FindObjectOfType<GameManager>();

        // initiate property values
        MaxHealth = health;
        Health = health;
        Speed = 1f;

        // initiate Dictionary to empty
        statusDisplays = new Dictionary<Status.StatusType, TMP_Text>();
        // fill statuses list with possible statuses (all the status types)
        statuses = new List<Status>
        {
            new Status(Status.StatusType.Clumsy, Color.green, 0, 0f, this),
            new Status(Status.StatusType.Wound, Color.red, 0, 0f, this),
            new Status(Status.StatusType.Stun, Color.yellow, 0, 0f, this),
            new Status(Status.StatusType.Poison, Color.magenta, 0, 0f, this),
            new Status(Status.StatusType.Shield, Color.blue, 0, 0f, this)
        };
        this.statusesParent = statusesParent;

        // add SetStatusDisplay(...) to StatusChangeEvent, so that whenever
        // the status is changed, the display is set.
        StatusChangeEvent += SetStatusDisplay;
    }

    /// <summary>
    /// Applies a status to this Entity.
    /// Stacks are added if the status already exists. Otherwise, the status
    /// effect/action is started.
    /// </summary>
    public void ApplyStatus(StatusModifier statusMod)
    {
        if (statusMod.type == Status.StatusType.None || statusMod.stacks == 0)
        {
            return;
        }

        Status status = GetStatus(statusMod.type);
        status.Stacks += statusMod.stacks;

        if (status.Stacks - statusMod.stacks == 0)
        {
            switch (statusMod.type)
            {
                case Status.StatusType.Clumsy:
                    Debug.LogWarning("Clumsy not implemented");
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

    /// <summary>
    /// Applies a stat modification to this Entity.
    /// </summary>
    public void ApplyStatModification(StatModifier statMod)
    {
        switch (statMod.type)
        {
            case Card.StatChange.None:
                break;

            case Card.StatChange.SetHealth:
                Health = statMod.value;
                MaxHealth = statMod.value;
                break;

            case Card.StatChange.IncreaseHealth:
                MaxHealth += statMod.value;
                break;

            case Card.StatChange.IncreaseSpeed:
                Speed += statMod.value / 100f;
                break;

            default:
                Debug.LogWarning("Card Stat Change not implemented");
                break;
        }
    }



    /// <summary>
    /// Safely returns the Status (containing all the status property/values)
    /// based on the StatusType from this Entity's list of statuses.
    /// </summary>
    private Status GetStatus(Status.StatusType type)
    {
        int i = (int) type - 1;
        if (i < 0 || i > statuses.Count)
        {
            return null;
        }
        return statuses[(int) type - 1];
    }

    /// <summary>
    /// Allows other scripts to trigger the ActionEvent of this Entity.
    /// </summary>
    public void TriggerActionEvent()
    {
        if (ActionEvent != null)
        {
            ActionEvent();
        }
    }

    /// <summary>
    /// Updates the status display when the status is changed.
    /// (This is added onto StatusChangeEvent when Entity is initialized.)
    /// </summary>
    private void SetStatusDisplay(Status.StatusType statusType, int oldStacks, int newStacks)
    {
        Status status = GetStatus(statusType);
        TMP_Text statusDisplay;
        // try get reference to text component if it exists
        statusDisplays.TryGetValue(statusType, out statusDisplay);

        // if adding stacks when previously there was none
        if (oldStacks < 1 && newStacks > 0)
        {
            // create the status icon (based on prefab) and attach to parent
            GameObject statusObject = Instantiate(gameManager.statusPrefab, statusesParent);
            // set image color and reference to text
            Image statusImage = statusObject.GetComponent<Image>();
            statusImage.color = status.color;
            statusDisplay = statusObject.GetComponentInChildren<TMP_Text>();
            statusDisplays.Add(statusType, statusDisplay);
        }
        // if removing stacks when previously there were stacks
        else if (oldStacks > 0 && newStacks < 1)
        {
            // remove reference to text component in the dictionary
            statusDisplays.Remove(statusType);
            // destroy the icon object
            Destroy(statusDisplay.transform.parent.gameObject);
        }

        if (statusDisplay != null)
        {
            // set text to number of stacks
            statusDisplay.text = newStacks.ToString();
        }
    }

    /// <summary>
    /// Trigger any HealthChange events if they exist.
    /// </summary>
    private void OnHealthChange(int oldHP, int newHP)
    {
        if (HealthChangeEvent != null)
        {
            HealthChangeEvent(oldHP, newHP);
        }
    }

    /// <summary>
    /// Trigger any SpeedChange events if they exist.
    /// </summary>
    private void OnSpeedChange(float oldSpeed, float newSpeed)
    {
        if (SpeedChangeEvent != null)
        {
            SpeedChangeEvent(oldSpeed, newSpeed);
        }
    }

    /// <summary>
    /// Trigger any MaxHealthChange events if they exist.
    /// </summary>
    private void OnMaxHealthChange(int oldHP, int newHP)
    {
        if (MaxHealthChangeEvent != null)
        {
            MaxHealthChangeEvent(oldHP, newHP);
        }
    }

    /// <summary>
    /// Trigger any StackChange events if they exist.
    /// </summary>
    private void OnStackChange(Status.StatusType statusType, int oldStacks, int newStacks)
    {
        if (StatusChangeEvent != null)
        {
            StatusChangeEvent(statusType, oldStacks, newStacks);
        }
    }

    /// <summary>
    /// Apply the Wound status effect onto this Entity.
    /// This status is triggered by ActionEvent.
    /// </summary>
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

    /// <summary>
    /// Apply the Stun status effect onto this Entity.
    /// This status is timer based.
    /// </summary>
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

    /// <summary>
    /// Apply the Poison status effect onto this Entity.
    /// This status is timer based.
    /// </summary>
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

    /// <summary>
    /// Apply the Shield status effect onto this Entity.
    /// This status is triggered on HealthChangeEvent.
    /// </summary>
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

}

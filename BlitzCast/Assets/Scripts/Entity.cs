using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Entity : MonoBehaviour
{
    public int hp;
    public int maxHP;
    public float speed;

    public delegate void Lorem();

    public event Lorem Event;

    public List<Status> statuses = new List<Status>();

    [SerializeField] private TMP_Text healthText;

    private GameManager gameManager;

    public void Initialize(int hp, int maxHP, float speed, List<Status> statuses, TMP_Text healthText)
    {
        this.hp = hp;
        this.maxHP = maxHP;
        this.speed = speed;
        this.statuses = statuses;
        this.healthText = healthText;
    }

    public int getHP()
    {
        return hp;
    }

    public void Damage (int h)
    {
        hp -= h;
    }

    public void Heal (int h)
    {
        hp = Mathf.Min(hp + h, maxHP);
    }

    public void IncreaseHP(int h)
    {
        maxHP += h;
        hp += h;
    }

    public void OnDoAction() {
        //handle Wounded
    }

    public void OnHealthChange()
    {

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

        GameObject statusObject = Instantiate(gameManager.statusPrefab);
        Image statusImage = statusObject.GetComponent<Image>();
        statusImage.color = color;
    }
}

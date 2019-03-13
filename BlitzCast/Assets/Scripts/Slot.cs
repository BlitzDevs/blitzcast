using UnityEngine;
using UnityEngine.UI;

public abstract class Slot : Selectable
{
    public int index;
    public GameObject slotObject;
    protected GameManager.Team team;


    public abstract void SetObject(GameObject slotObject);
    protected abstract void Initialize();

    protected override void Start()
    {
        Initialize();
    }


    public void SetTeam(GameManager.Team team)
    {
        this.team = team;
    }

    public GameManager.Team GetTeam()
    {
        return team;
    }

}

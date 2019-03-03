using UnityEngine;

public abstract class Slot : MonoBehaviour
{
    public int index;
    public GameObject slotObject;

    protected GameManager.Team team;


    public abstract void SetObject(GameObject slotObject);

    public void SetTeam(GameManager.Team team)
    {
        this.team = team;
    }

    public GameManager.Team GetTeam()
    {
        return team;
    }

}

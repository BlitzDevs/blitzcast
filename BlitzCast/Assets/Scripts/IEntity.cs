using System.Collections;

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
public interface IEntity
{
    void Damage(int hp);
    void Heal(int hp);
    void DoStatuses();
    int GetHealth();
    void ApplyStatus(Card.StatusType statusType, int stacks);
}

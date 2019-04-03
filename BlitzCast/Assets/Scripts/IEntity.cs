using System.Collections;

public interface IEntity
{
    void Damage(int hp);
    void Heal(int hp);
    int GetHealth();
    IEnumerator ExecuteStatuses();
}

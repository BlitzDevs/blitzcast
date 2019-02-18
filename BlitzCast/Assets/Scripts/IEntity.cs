using UnityEngine;

public interface IEntity {

    void Damage(int hp);
    void Heal(int hp);
    void SetHealth(int hp);
    Vector3Int GetStats();

}

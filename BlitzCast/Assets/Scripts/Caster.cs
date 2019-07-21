using UnityEngine;

/// <summary>
/// Contains all the properties for a Caster, which is avatar/class-like and
/// can grant the player a unique passive.
/// </summary>
[CreateAssetMenu(fileName = "New Caster", menuName = "Caster")]
public class Caster : ScriptableObject {

    public string casterName;
    public float spriteAnimateSpeed;


    //TODO: Fully implement the "passive" effect
    // public void DoYourThing() ?
}

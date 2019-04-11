using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Caster", menuName = "Caster")]
public class Caster : ScriptableObject {

    public string casterName = "???";
    public float spriteAnimateSpeed = 30f;
    public Color color = Color.white;

    // unique passive for different Casters
    // somewhat like an avatar/class

    // public void DoYourThing() ?
}

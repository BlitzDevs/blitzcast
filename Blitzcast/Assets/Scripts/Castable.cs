using UnityEngine;

public abstract class Castable : ScriptableObject
{
    public abstract void Cast(CreatureCard target);
    public abstract void Cast(SpellCard target);
    public abstract void Cast(Player target);
}

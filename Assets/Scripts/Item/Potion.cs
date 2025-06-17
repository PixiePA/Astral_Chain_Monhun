using UnityEngine;

public class Potion : Item
{
    protected override void ActivateEffect()
    {
        StatsEvents.Damage(-10); // Heal 10 health
    }
}

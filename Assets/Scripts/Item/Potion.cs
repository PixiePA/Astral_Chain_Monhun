using UnityEngine;

public class Potion : Item
{
    protected override void ActivateEffect()
    {
        PlayerEvents.ChangeHealth(10); // Heal 10 health
    }
}

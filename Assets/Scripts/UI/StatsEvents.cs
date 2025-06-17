using System;
using UnityEngine;

public static class StatsEvents
{
    public static Action<float> OnDamage;
    public static void Damage(float changeAmount)
    {
        OnDamage?.Invoke(changeAmount);
    }

    public static Action<float> OnHealthChanged;
    public static void HealthUI(float newHealth)
    {
        OnHealthChanged?.Invoke(newHealth);
    }
    public static Action<float> OnEnergyChanged;
    public static void ChangeEnergy(float newEnergy)
    {
        OnEnergyChanged?.Invoke(newEnergy);
    }
}

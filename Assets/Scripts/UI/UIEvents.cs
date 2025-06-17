using System;
using UnityEngine;

public static class UIEvents

{
    public static Action<float> OnHealthChanged;
    public static void HealthChanged(float newHealth)
    {
        OnHealthChanged?.Invoke(newHealth);
    }
    public static Action<float> OnEnergyChanged;
    public static void EnergyChanged(float newEnergy)
    {
        OnEnergyChanged?.Invoke(newEnergy);
    }
}

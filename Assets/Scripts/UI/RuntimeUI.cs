using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RuntimeUI : MonoBehaviour
{
    public StatsController statsController;
    public UIDocument uiDocument;

    private VisualElement healthbarBackground;
    private VisualElement healthbarFill;
    private VisualElement energybarBackground;
    private VisualElement energybarFill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void HealthChanged(float newHealth)
    {
        healthbarBackground.style.width = 650 * (statsController.GetMaxHealth() / 100); // Scale based on max health
        float healthRatio = newHealth / statsController.GetMaxHealth();
        float healthPercentage = Mathf.Lerp(5, 100, healthRatio);
        healthbarFill.style.width = Length.Percent(healthPercentage);
    }
    void EnergyChanged(float newEnergy)
    {
        energybarBackground.style.width = 535 * (statsController.GetMaxEnergy() / 100); // Scale based on max energy
        float energyRatio = newEnergy / statsController.GetMaxEnergy();
        float energyPercentage = Mathf.Lerp(5, 100, energyRatio);
        energybarFill.style.width = Length.Percent(energyPercentage);
    }
    private void OnEnable()
    {
        UIEvents.OnHealthChanged += HealthChanged;
        UIEvents.OnEnergyChanged += EnergyChanged;
        healthbarBackground = uiDocument.rootVisualElement.Q<VisualElement>("HealthbarBackground");
        healthbarFill = uiDocument.rootVisualElement.Q<VisualElement>("HealthbarMask");
        energybarBackground = uiDocument.rootVisualElement.Q<VisualElement>("EnergybarBackground");
        energybarFill = uiDocument.rootVisualElement.Q<VisualElement>("EnergybarMask");
    }
    private void OnDisable()
    {
        UIEvents.OnHealthChanged -= HealthChanged;
        UIEvents.OnEnergyChanged -= EnergyChanged;
    }
}

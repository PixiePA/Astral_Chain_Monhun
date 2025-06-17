using System;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [SerializeField] private bool testButton = false;
    [SerializeField] private float health = 100; //temp
    [SerializeField] private float maxHealth = 100;

    [SerializeField] private float energy = 100;
    [SerializeField] private float maxEnergy = 100;

    public Action<float> OnHealthChanged;
    public void ChangeHealth(float changeAmount)
    {
        health = Mathf.Clamp(health + changeAmount, 0, maxHealth);
        OnHealthChanged?.Invoke(health);
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetMaxEnergy()
    {
        return maxEnergy;
    }

    public Action<float> OnEnergyChanged;
    public void ChangeEnergy(float changeAmount)
    {
        energy = Mathf.Clamp(energy + changeAmount, 0, maxEnergy);
        OnEnergyChanged?.Invoke(energy);
    }

    public Action<ItemScriptableObject> OnPickup;
    public void PickupItem(ItemScriptableObject item)
    {
        OnPickup?.Invoke(item);
    }

    public Action<ItemScriptableObject> OnUseItem;
    public void UseItem(ItemScriptableObject item)
    {
        OnUseItem?.Invoke(item);
    }

    void Start()
    {
        
    }

    // Debug
    void Update()
    {
        ChangeEnergy(0.1f);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeHealth(-10);
            ChangeEnergy(-20);
            testButton = false; // Reset the button state after triggering
        }
    }
}

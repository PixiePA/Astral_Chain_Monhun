using System;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [SerializeField] private bool testButton = false;
    [SerializeField] private float health = 100;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeEnergy(0.1f); // Example of changing energy over time
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeHealth(-10);
            ChangeEnergy(-20);
            testButton = false; // Reset the button state after triggering
        }
    }
}

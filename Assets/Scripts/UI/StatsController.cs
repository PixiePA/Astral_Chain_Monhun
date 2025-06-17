using System;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [SerializeField] private float health = 100; //temp
    [SerializeField] private float maxHealth = 100;

    [SerializeField] private float energy = 100;
    [SerializeField] private float maxEnergy = 100;

    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetMaxEnergy()
    {
        return maxEnergy;
    }

    void TakeDamage(float healthChange)
    {         
        health = Mathf.Clamp(health + healthChange, 0, maxHealth);
        UIEvents.HealthChanged(health);
    }

    // Update is called once per frame
    void Update()
    {
        energy += Time.deltaTime * 5;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        UIEvents.EnergyChanged(energy);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerEvents.ChangeHealth(-10);
            energy = Mathf.Clamp(energy - 10, 0, maxEnergy);
            UIEvents.EnergyChanged(energy);
        }
    }
    private void OnEnable()
    {
        PlayerEvents.onChangeHealth += TakeDamage;
    }
    
    private void OnDisable()
    {
        PlayerEvents.onChangeHealth -= TakeDamage;
    }
}

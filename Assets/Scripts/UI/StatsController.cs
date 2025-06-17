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

    void TakeDamage(float damage)
    {         
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        StatsEvents.HealthUI(health);
    }

    // Update is called once per frame
    void Update()
    {
        energy += Time.deltaTime * 5;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        StatsEvents.ChangeEnergy(energy);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StatsEvents.Damage(10);
            energy = Mathf.Clamp(energy - 10, 0, maxEnergy);
            StatsEvents.ChangeEnergy(energy);
        }
    }
    private void OnEnable()
    {
        StatsEvents.OnDamage += TakeDamage;
    }
    
    private void OnDisable()
    {
        StatsEvents.OnDamage -= TakeDamage;
    }
}

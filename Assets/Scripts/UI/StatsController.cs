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

    void TakeDamage(float damage)
    public Action<ItemScriptableObject> OnUseItem;
    public void UseItem(ItemScriptableObject item)
    {
        OnUseItem?.Invoke(item);
    }

    void Start()
    public Action<ItemScriptableObject> OnUseItem;
        energy += Time.deltaTime * 5;
    {
        OnUseItem?.Invoke(item);
    }

    void Start()
    public Action<ItemScriptableObject> OnUseItem;
    public void UseItem(ItemScriptableObject item)
    {
        OnUseItem?.Invoke(item);
    }

    void Start()
    public Action<ItemScriptableObject> OnUseItem;
    public void UseItem(ItemScriptableObject item)
    {
        OnUseItem?.Invoke(item);
    }

    void Start()
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        StatsEvents.HealthUI(health);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeEnergy(0.1f);
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

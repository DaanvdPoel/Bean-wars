using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour //Daan
{
    public float health;
    private float maxHealth;
    private HealthBar healthBar;
    public SimpleBehaviorTree.Examples.UnitSpawner unitSpawner;

    private void Awake()
    {
        healthBar = gameObject.GetComponent<HealthBar>();
    }

    /// <summary>
    /// sets the maxhealth for the Unit
    /// </summary>
    /// <param name="_maxHealth"></param>
    public void SetMaxHealth(float _maxHealth)
    {
        maxHealth = _maxHealth;
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    /// <summary>
    /// removes or adds health to the Unit
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeHealth(float amount)
    {
        health += amount;
        healthBar.SetHealth(health);
        CheckHealth();
    }

    /// <summary>
    /// checks if the enemy issent dead
    /// </summary>
    private void CheckHealth()
    {
        if(health <= 0)
        {
            unitSpawner.RemoveUnitFromList(gameObject);
            Destroy(gameObject, 2f);
            gameObject.SetActive(false);
        }
    }
}

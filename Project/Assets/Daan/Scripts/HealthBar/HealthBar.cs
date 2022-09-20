using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour //daan
{
    [SerializeField] private Slider healthBar;

    /// <summary>
    /// sets the max value of the health bar slider
    /// </summary>
    /// <param name="maxHealth"></param>
    public void SetMaxHealth(float maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }

    /// <summary>
    /// chanches the health bar slider value
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(float health)
    {
        healthBar.value = health;
    }
}

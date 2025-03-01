using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI value;

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        value.text = maxHealth.ToString();
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        value.text = health.ToString();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider healthSlider;

    public void UpdateHealthBar(int newHP)
    {
        healthSlider.value = newHP;
    }
    public void setMaxHealth(int maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = maxHP;
    }

}

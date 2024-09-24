using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] Slider health_slider;

    public void setHealth(float health) => health_slider.value = health;

    public void setMaxHealth(float health)
    {
        health_slider.maxValue = health;
        health_slider.value = health;
    }
}

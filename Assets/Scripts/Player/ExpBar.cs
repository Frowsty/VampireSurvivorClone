using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expbar : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] Slider exp_slider;

    /*
     * PUBLIC FUNCTIONS
     */
    public void setExp(int experience) => exp_slider.value = experience;

    public void setMaxExp(int experience) => exp_slider.maxValue = experience;
}
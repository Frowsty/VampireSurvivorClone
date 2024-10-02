using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    /*
     * PRIVATE VARIABLES
     */
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI value;
    
    /*
     * PRIVATE FUNCTIONS
     */
    void Start()
    {
        slider.onValueChanged.AddListener(delegate
        {
            value.text = slider.value.ToString();
        });
    }
}

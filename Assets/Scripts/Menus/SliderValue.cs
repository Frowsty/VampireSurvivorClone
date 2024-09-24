using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI value;
    
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(delegate
        {
            value.text = slider.value.ToString();
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}

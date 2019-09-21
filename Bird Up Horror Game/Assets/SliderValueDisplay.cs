using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    public bool isFloat;

    [SerializeField]
    private Text valueText;

    private void Update()
    {
        if(isFloat)
            valueText.text = GetComponent<Slider>().value.ToString("F2");
        else
            valueText.text = GetComponent<Slider>().value.ToString();
    }
}

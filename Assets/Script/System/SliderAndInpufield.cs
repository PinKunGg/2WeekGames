using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderAndInpufield : MonoBehaviour
{
    public float sliderValue;
    public Slider _slider;
    public TMP_InputField _inputField;

    public void OnSliderValueChange(){
        sliderValue = float.Parse(_slider.value.ToString());
        _inputField.text = sliderValue.ToString();
    }

    public void OnInputFieldValueChange_InputField(){
        if(float.Parse(_inputField.text) > _slider.maxValue){
            sliderValue = _slider.maxValue;
            _inputField.text = sliderValue.ToString();
        }
        else if(float.Parse(_inputField.text) < _slider.minValue){
            sliderValue = _slider.minValue;
            _inputField.text = sliderValue.ToString();
        }
        else{
            sliderValue = float.Parse(_inputField.text);
        }

        _slider.value = sliderValue;
    }
}

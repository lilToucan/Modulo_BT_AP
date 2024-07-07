using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI speedAmount;
    float sliderAmount;

    public void OnSliderValueChange(Slider slider)
    {
        sliderAmount = Mathf.Lerp(1, 5, slider.value / slider.maxValue);
        UpdateTxt();
        Time.timeScale = sliderAmount;
    }

    private void UpdateTxt()
    {
        var txt = sliderAmount.ToString("Speed 0.0");
        speedAmount.text = $"{txt}x";
    }
}

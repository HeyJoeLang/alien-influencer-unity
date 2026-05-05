using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class SoundSettingsSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text tmpText;
    public EventReference eventReference;
    public FMOD.Studio.EventInstance fmodEventInstance;
    public string parameterName;

    void Start()
    {
        // Get initial value from the event instance parameter
        float initialValue;
        fmodEventInstance.getParameterByName(parameterName, out initialValue);
        Debug.Log($"[SoundSettingsSlider] Initial value for parameter '{parameterName}': {initialValue}");

        // Set slider and text to initial value
        slider.value = initialValue;
        tmpText.text = initialValue.ToString("F2");
        Debug.Log($"[SoundSettingsSlider] Slider and text set to: {initialValue}");

        // Add listener for slider value changes
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        Debug.Log($"[SoundSettingsSlider] Slider value changed to: {value}");

        // Update the parameter on the event instance
        fmodEventInstance.setParameterByName(parameterName, value);
        Debug.Log($"[SoundSettingsSlider] FMOD parameter '{parameterName}' updated to: {value}");

        // Update the text label
        tmpText.text = value.ToString("F2");
    }
}

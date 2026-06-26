using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsSlider : MonoBehaviour
{
    public Slider slider;
    public string volumeParameterName = "Volume";

    private void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        // Reserved for future Unity AudioMixer group volume control.
    }
}

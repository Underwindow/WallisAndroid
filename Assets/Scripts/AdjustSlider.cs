using UnityEngine;
using UnityEngine.UI;

public class AdjustSlider : MonoBehaviour
{
    [HideInInspector]
    public float defaultValue;
    public string valueName;
    private Slider slider;

    // Start is called before the first frame update
    public void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat(valueName, defaultValue);
    }

    public void ValueToDefault()
    {
        slider.value = defaultValue;
    }

    public void SetMinValue()
    {
        slider.value = slider.minValue;
    }

    public void SetMaxValue()
    {
        slider.value = slider.maxValue;
    }
}

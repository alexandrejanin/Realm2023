using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputSlider : MonoBehaviour {
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Slider slider;

    [SerializeField] private int min, max, initialValue;

    public int Value => Mathf.RoundToInt(Mathf.Lerp(min, max, slider.value));

    private void Awake() {
        inputField.onEndEdit.AddListener(text => SetValue(int.Parse(text)));
        slider.onValueChanged.AddListener(x => SetValue(Mathf.RoundToInt(Mathf.Lerp(min, max, x))));

        SetValue(initialValue);
    }

    public void SetValue(int value) {
        value = Mathf.Clamp(value, min, max);
        slider.value = (value - min) / (float)(max - min);
        inputField.text = value.ToString();
    }
}
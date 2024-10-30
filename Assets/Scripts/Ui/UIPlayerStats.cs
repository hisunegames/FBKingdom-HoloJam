using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider manaSlider;

    void Start()
    {
        PlayerScript.instance.EventHealthUpdate += OnHealthUpdate;
        PlayerScript.instance.EventManaUpdate += OnManaUpdate;
    }

    private void OnHealthUpdate(float value, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = value;
    }

    private void OnManaUpdate(float value, float max)
    {
        manaSlider.maxValue = max;
        manaSlider.value = value;
    }
}

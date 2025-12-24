using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    private void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        int safety = 10;
        while (AudioManager.Instance == null && safety-- > 0)
            yield return null;

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager.Instance is not in scene");
            yield break;
        }

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        slider.SetValueWithoutNotify(AudioManager.Instance.GetVolume());
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolume(value);
    }
}

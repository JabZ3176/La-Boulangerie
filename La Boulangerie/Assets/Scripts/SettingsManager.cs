using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public Slider audioSlider;
    public AudioSource musicSource;

    [Header("Brightness")]
    public Slider brightnessSlider;
    public CanvasGroup darkOverlay;
    public CanvasGroup lightOverlay;

    public GameObject container;
    public GameObject SettingsContainter;

    private float baseVolume; // your 0.02

    void Awake()
    {
        // Store the volume you set in Inspector (0.02)
        baseVolume = musicSource.volume;
    }

    void Start()
    {
        baseVolume = musicSource.volume;

        float savedAudio = PlayerPrefs.GetFloat("Audio", 50f);
        audioSlider.SetValueWithoutNotify(savedAudio);
        ApplyAudio(savedAudio);

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 50f);
        brightnessSlider.SetValueWithoutNotify(savedBrightness);
        ApplyBrightness(savedBrightness);

        audioSlider.onValueChanged.AddListener(ApplyAudio);
        brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
    }

    public void ApplyAudio(float value)
    {
        // Convert slider (0–100) into multiplier
        float multiplier = value / 50f;

        // 50 = 1x (your base volume)
        // 100 = 2x louder
        // 0 = silent
        musicSource.volume = baseVolume * multiplier;

        PlayerPrefs.SetFloat("Audio", value);
    }

    public void ApplyBrightness(float value)
    {
        if (value < 50f)
        {
            float t = 1f - (value / 50f);
            darkOverlay.alpha = Mathf.Lerp(0f, 0.6f, t);
            lightOverlay.alpha = 0f;
        }
        else
        {
            float t = (value - 50f) / 50f;
            lightOverlay.alpha = Mathf.Lerp(0f, 0.4f, t);
            darkOverlay.alpha = 0f;
        }

        PlayerPrefs.SetFloat("Brightness", value);
    }

    public void BackButton()
    {
        container.SetActive(true);
        SettingsContainter.SetActive(false);
    }

}
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region AUDIO

    [Header("Audio")]
    public Slider audioSlider;

    #endregion

    #region BRIGHTNESS

    [Header("Brightness")]
    public Slider brightnessSlider;
    public CanvasGroup darkOverlay;
    public CanvasGroup lightOverlay;

    #endregion

    #region REFERENCES

    [Header("References")]
    public GameObject container;
    public GameObject SettingsContainter;

    #endregion

    #region START

    void Start()
    {
        float savedAudio = PlayerPrefs.GetFloat("Audio", 50f);
        audioSlider.SetValueWithoutNotify(savedAudio);
        ApplyAudio(savedAudio);

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 50f);
        brightnessSlider.SetValueWithoutNotify(savedBrightness);
        ApplyBrightness(savedBrightness);

        audioSlider.onValueChanged.AddListener(ApplyAudio);
        brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
    }

    #endregion

    #region AUDIO

    public void ApplyAudio(float value)
    {
        float normalized = value / 100f;

        float musicVolume = normalized * 0.08f;
        float sfxVolume = normalized * 1f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(musicVolume);
            SoundManager.Instance.SetSFXVolume(sfxVolume);
        }

        PlayerPrefs.SetFloat("Audio", value);
        PlayerPrefs.Save();
    }

    #endregion

    #region BRIGHTNESS

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
        PlayerPrefs.Save();
    }

    #endregion

    #region BUTTONS

    public void BackButton()
    {
        container.SetActive(true);
        SettingsContainter.SetActive(false);
    }

    #endregion
}
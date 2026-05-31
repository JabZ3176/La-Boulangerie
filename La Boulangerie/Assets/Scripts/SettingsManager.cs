using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region AUDIO SLIDERS
    [Header("Audio Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
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
    public GameObject settingsContainer;
    #endregion

    #region START
    void Start()
    {
        // load saved values or use defaults
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 50f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 50f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 50f);
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 50f);

        // set slider values without triggering callbacks yet
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(savedMaster);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(savedMusic);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(savedSFX);
        if (brightnessSlider != null) brightnessSlider.SetValueWithoutNotify(savedBrightness);

        // apply saved values immediately
        ApplyMaster(savedMaster);
        ApplyMusic(savedMusic);
        ApplySFX(savedSFX);
        ApplyBrightness(savedBrightness);

        // hook up listeners
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(ApplyMaster);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(ApplyMusic);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(ApplySFX);
        if (brightnessSlider != null) brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
    }
    #endregion

    #region MASTER VOLUME
    public void ApplyMaster(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMasterVolume(value / 100f);

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }
    #endregion

    #region MUSIC VOLUME
    public void ApplyMusic(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicVolumeOnly(value / 100f);

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
    #endregion

    #region SFX VOLUME
    public void ApplySFX(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolumeOnly(value / 100f);

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
    #endregion

    #region BRIGHTNESS
    public void ApplyBrightness(float value)
    {
        if (value < 50f)
        {
            float t = 1f - (value / 50f);
            if (darkOverlay != null) darkOverlay.alpha = Mathf.Lerp(0f, 0.6f, t);
            if (lightOverlay != null) lightOverlay.alpha = 0f;
        }
        else
        {
            float t = (value - 50f) / 50f;
            if (lightOverlay != null) lightOverlay.alpha = Mathf.Lerp(0f, 0.4f, t);
            if (darkOverlay != null) darkOverlay.alpha = 0f;
        }

        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }
    #endregion

    #region BUTTONS
    public void BackButton()
    {
        if (container != null) container.SetActive(true);
        if (settingsContainer != null) settingsContainer.SetActive(false);
    }
    #endregion
}
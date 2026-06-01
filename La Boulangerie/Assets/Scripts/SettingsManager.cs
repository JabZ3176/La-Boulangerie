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

    #region PLAYER PREF KEYS
    private const string BrightnessKey = "Brightness";
    #endregion

    #region UNITY
    private void Start()
    {
        SetupSlider(masterSlider, 0f, 100f);
        SetupSlider(musicSlider, 0f, 100f);
        SetupSlider(sfxSlider, 0f, 100f);
        SetupSlider(brightnessSlider, 0f, 100f);

        float savedMaster = PlayerPrefs.GetFloat(SoundManager.MasterVolumeKey, 50f);
        float savedMusic = PlayerPrefs.GetFloat(SoundManager.MusicVolumeKey, 50f);
        float savedSFX = PlayerPrefs.GetFloat(SoundManager.SFXVolumeKey, 50f);
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 50f);

        // Backwards compatibility with your old single Audio key.
        if (!PlayerPrefs.HasKey(SoundManager.MasterVolumeKey) && PlayerPrefs.HasKey("Audio"))
        {
            float oldAudio = Mathf.Clamp(PlayerPrefs.GetFloat("Audio", 50f), 0f, 100f);
            savedMaster = oldAudio;
            savedMusic = oldAudio;
            savedSFX = oldAudio;
        }

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(savedMaster);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(savedMusic);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(savedSFX);
        if (brightnessSlider != null) brightnessSlider.SetValueWithoutNotify(savedBrightness);

        ApplyMaster(savedMaster);
        ApplyMusic(savedMusic);
        ApplySFX(savedSFX);
        ApplyBrightness(savedBrightness);

        if (masterSlider != null)
        {
            masterSlider.onValueChanged.RemoveListener(ApplyMaster);
            masterSlider.onValueChanged.AddListener(ApplyMaster);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(ApplyMusic);
            musicSlider.onValueChanged.AddListener(ApplyMusic);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(ApplySFX);
            sfxSlider.onValueChanged.AddListener(ApplySFX);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveListener(ApplyBrightness);
            brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
        }
    }
    #endregion

    #region SETUP
    private void SetupSlider(Slider slider, float min, float max)
    {
        if (slider == null) return;

        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = false;
    }
    #endregion

    #region AUDIO
    public void ApplyMaster(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);
        PlayerPrefs.SetFloat(SoundManager.MasterVolumeKey, value);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMasterVolume(value / 100f);
    }

    public void ApplyMusic(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);
        PlayerPrefs.SetFloat(SoundManager.MusicVolumeKey, value);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicVolumeOnly(value / 100f);
    }

    public void ApplySFX(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);
        PlayerPrefs.SetFloat(SoundManager.SFXVolumeKey, value);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolumeOnly(value / 100f);
    }
    #endregion

    #region BRIGHTNESS
    public void ApplyBrightness(float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);

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

        PlayerPrefs.SetFloat(BrightnessKey, value);
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

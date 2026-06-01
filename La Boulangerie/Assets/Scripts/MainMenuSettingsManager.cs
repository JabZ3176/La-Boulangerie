using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main-menu-only settings controller.
///
/// Use this on the Main Menu scene instead of the pause-menu SettingsManager.
/// It opens/closes a settings panel with buttons and does not return to the pause menu.
/// </summary>
public class MainMenuSettingsManager : MonoBehaviour
{
    #region PLAYER PREF KEYS
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SFXVolume";
    private const string BrightnessKey = "Brightness";

    // Optional backwards compatibility with older SoundManager versions that read "Audio".
    private const string LegacyAudioKey = "Audio";
    #endregion

    #region PANELS
    [Header("Panels")]
    [Tooltip("The main menu container. Optional, but recommended if you want it hidden while settings are open.")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("The settings panel/container that should open and close from buttons.")]
    [SerializeField] private GameObject settingsPanel;

    [Tooltip("If enabled, opening settings hides the main menu panel, and closing settings shows it again.")]
    [SerializeField] private bool hideMainMenuWhileSettingsOpen = true;

    [Tooltip("Usually false. Enable only if you want the settings panel visible when the scene starts.")]
    [SerializeField] private bool showSettingsOnStart = false;
    #endregion

    #region AUDIO SLIDERS
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    #endregion

    #region BRIGHTNESS
    [Header("Brightness")]
    [SerializeField] private Slider brightnessSlider;

    [Tooltip("Black overlay used to darken the screen when brightness is below 50.")]
    [SerializeField] private CanvasGroup darkOverlay;

    [Tooltip("White overlay used to brighten the screen when brightness is above 50.")]
    [SerializeField] private CanvasGroup lightOverlay;

    [SerializeField] private float maxDarkOverlayAlpha = 0.6f;
    [SerializeField] private float maxLightOverlayAlpha = 0.4f;
    #endregion

    #region DEFAULTS
    [Header("Defaults")]
    [Range(0f, 100f)][SerializeField] private float defaultMasterVolume = 50f;
    [Range(0f, 100f)][SerializeField] private float defaultMusicVolume = 50f;
    [Range(0f, 100f)][SerializeField] private float defaultSfxVolume = 50f;
    [Range(0f, 100f)][SerializeField] private float defaultBrightness = 50f;
    #endregion

    #region UNITY
    private void Awake()
    {
        AddSliderListeners();
    }

    private void Start()
    {
        LoadSlidersFromPrefs();
        ApplyAllSettingsWithoutSavingAgain();

        if (settingsPanel != null)
            settingsPanel.SetActive(showSettingsOnStart);

        if (mainMenuPanel != null && hideMainMenuWhileSettingsOpen)
            mainMenuPanel.SetActive(!showSettingsOnStart);

        ShowCursorForMenu();
    }

    private void OnDestroy()
    {
        RemoveSliderListeners();
    }
    #endregion

    #region BUTTON METHODS
    /// <summary>
    /// Assign this to the Settings button in the Main Menu.
    /// </summary>
    public void OpenSettings()
    {
        LoadSlidersFromPrefs();
        ApplyAllSettingsWithoutSavingAgain();

        if (mainMenuPanel != null && hideMainMenuWhileSettingsOpen)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        ShowCursorForMenu();
    }

    /// <summary>
    /// Assign this to the Back button inside the settings panel.
    /// This only closes the settings panel and returns to the main menu panel.
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainMenuPanel != null && hideMainMenuWhileSettingsOpen)
            mainMenuPanel.SetActive(true);

        ShowCursorForMenu();
    }

    public void ToggleSettings()
    {
        bool settingsOpen = settingsPanel != null && settingsPanel.activeSelf;

        if (settingsOpen)
            CloseSettings();
        else
            OpenSettings();
    }
    #endregion

    #region SLIDER CALLBACKS
    public void ApplyMaster(float sliderValue)
    {
        sliderValue = ClampSliderValue(sliderValue);
        PlayerPrefs.SetFloat(MasterVolumeKey, sliderValue);
        PlayerPrefs.SetFloat(LegacyAudioKey, sliderValue);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMasterVolume(sliderValue / 100f);
    }

    public void ApplyMusic(float sliderValue)
    {
        sliderValue = ClampSliderValue(sliderValue);
        PlayerPrefs.SetFloat(MusicVolumeKey, sliderValue);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicVolumeOnly(sliderValue / 100f);
    }

    public void ApplySFX(float sliderValue)
    {
        sliderValue = ClampSliderValue(sliderValue);
        PlayerPrefs.SetFloat(SfxVolumeKey, sliderValue);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolumeOnly(sliderValue / 100f);
    }

    public void ApplyBrightness(float sliderValue)
    {
        sliderValue = ClampSliderValue(sliderValue);
        PlayerPrefs.SetFloat(BrightnessKey, sliderValue);
        PlayerPrefs.Save();

        ApplyBrightnessVisuals(sliderValue);
    }
    #endregion

    #region RESET
    public void ResetSettingsToDefaults()
    {
        SetSliderValue(masterSlider, defaultMasterVolume);
        SetSliderValue(musicSlider, defaultMusicVolume);
        SetSliderValue(sfxSlider, defaultSfxVolume);
        SetSliderValue(brightnessSlider, defaultBrightness);

        ApplyMaster(defaultMasterVolume);
        ApplyMusic(defaultMusicVolume);
        ApplySFX(defaultSfxVolume);
        ApplyBrightness(defaultBrightness);
    }
    #endregion

    #region INTERNAL
    private void AddSliderListeners()
    {
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(ApplyMaster);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(ApplyMusic);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(ApplySFX);
        if (brightnessSlider != null) brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
    }

    private void RemoveSliderListeners()
    {
        if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(ApplyMaster);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(ApplyMusic);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(ApplySFX);
        if (brightnessSlider != null) brightnessSlider.onValueChanged.RemoveListener(ApplyBrightness);
    }

    private void LoadSlidersFromPrefs()
    {
        SetSliderValue(masterSlider, PlayerPrefs.GetFloat(MasterVolumeKey, defaultMasterVolume));
        SetSliderValue(musicSlider, PlayerPrefs.GetFloat(MusicVolumeKey, defaultMusicVolume));
        SetSliderValue(sfxSlider, PlayerPrefs.GetFloat(SfxVolumeKey, defaultSfxVolume));
        SetSliderValue(brightnessSlider, PlayerPrefs.GetFloat(BrightnessKey, defaultBrightness));
    }

    private void ApplyAllSettingsWithoutSavingAgain()
    {
        float master = masterSlider != null ? masterSlider.value : PlayerPrefs.GetFloat(MasterVolumeKey, defaultMasterVolume);
        float music = musicSlider != null ? musicSlider.value : PlayerPrefs.GetFloat(MusicVolumeKey, defaultMusicVolume);
        float sfx = sfxSlider != null ? sfxSlider.value : PlayerPrefs.GetFloat(SfxVolumeKey, defaultSfxVolume);
        float brightness = brightnessSlider != null ? brightnessSlider.value : PlayerPrefs.GetFloat(BrightnessKey, defaultBrightness);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMasterVolume(master / 100f);
            SoundManager.Instance.SetMusicVolumeOnly(music / 100f);
            SoundManager.Instance.SetSFXVolumeOnly(sfx / 100f);
        }

        ApplyBrightnessVisuals(brightness);
    }

    private void ApplyBrightnessVisuals(float sliderValue)
    {
        sliderValue = ClampSliderValue(sliderValue);

        if (sliderValue < 50f)
        {
            float t = 1f - (sliderValue / 50f);
            if (darkOverlay != null) darkOverlay.alpha = Mathf.Lerp(0f, maxDarkOverlayAlpha, t);
            if (lightOverlay != null) lightOverlay.alpha = 0f;
        }
        else
        {
            float t = (sliderValue - 50f) / 50f;
            if (lightOverlay != null) lightOverlay.alpha = Mathf.Lerp(0f, maxLightOverlayAlpha, t);
            if (darkOverlay != null) darkOverlay.alpha = 0f;
        }
    }

    private void SetSliderValue(Slider slider, float value)
    {
        if (slider == null) return;
        slider.SetValueWithoutNotify(ClampSliderValue(value));
    }

    private float ClampSliderValue(float value)
    {
        return Mathf.Clamp(value, 0f, 100f);
    }

    private void ShowCursorForMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    #region SINGLETON
    public static SoundManager Instance;
    #endregion

    #region SCENE MUSIC
    [System.Serializable]
    public class SceneMusic
    {
        [Tooltip("The exact Unity scene name. Example: Level1, Level2, Shop, BossLevel")]
        public string sceneName;

        [Tooltip("The music clip that should play when this scene loads.")]
        public AudioClip musicClip;
    }

    [Header("Scene Music List")]
    [Tooltip("Add future level music here. You do not need to edit this script for new levels.")]
    public List<SceneMusic> sceneMusic = new List<SceneMusic>();

    [Header("Music Behaviour")]
    [SerializeField] private bool playMusicOnSceneLoad = true;
    [SerializeField] private bool restartSameClip = false;
    [SerializeField] private float fadeDuration = 1f;
    #endregion

    #region UI SOUNDS
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    #endregion

    #region GAME SOUNDS
    [Header("Game Sounds")]
    public AudioClip baguetteThrow;
    public AudioClip enemyDamage;
    public AudioClip enemyHurt;
    public AudioClip playerFall;
    public AudioClip playerFallHit;
    public AudioClip fireLoop;
    public AudioClip fireHurt;
    public AudioClip playerJump;
    public AudioClip spikeHit;
    public AudioClip playerHurt;
    #endregion

    #region LEGACY MUSIC FIELDS
    [Header("Legacy Music Fields - optional fallback")]
    [Tooltip("These are kept so your current assignments still work. New music should go in Scene Music List above.")]
    public AudioClip mainMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip tutorialMusic;
    public AudioClip levelSceneMusic;
    #endregion

    #region VOLUME SETTINGS
    public const string MasterVolumeKey = "MasterVolume";
    public const string MusicVolumeKey = "MusicVolume";
    public const string SFXVolumeKey = "SFXVolume";

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float masterVolume = 0.5f;
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.5f;

    [Tooltip("Keeps your music quieter than SFX even when sliders are high.")]
    [Range(0f, 1f)][SerializeField] private float musicOutputMultiplier = 0.08f;
    [Range(0f, 2f)][SerializeField] private float sfxOutputMultiplier = 1f;
    #endregion

    #region PRIVATE VARIABLES
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Coroutine fadeCoroutine;
    private readonly Dictionary<string, AudioClip> musicByScene = new Dictionary<string, AudioClip>();
    #endregion

    #region UNITY
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateAudioSources();
        LoadVolumes();
        RebuildMusicLookup();
        ApplySourceVolumes();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (playMusicOnSceneLoad)
            PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region SETUP
    private void CreateAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    private void LoadVolumes()
    {
        // New consistent keys are 0-100 because your SettingsManager sliders use 0-100.
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 50f) / 100f;
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 50f) / 100f;
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 50f) / 100f;

        // Backwards compatibility with your old single "Audio" key.
        if (!PlayerPrefs.HasKey(MasterVolumeKey) && PlayerPrefs.HasKey("Audio"))
        {
            float oldAudio = Mathf.Clamp(PlayerPrefs.GetFloat("Audio", 50f), 0f, 100f) / 100f;
            masterVolume = oldAudio;
            musicVolume = oldAudio;
            sfxVolume = oldAudio;
            SaveAllVolumes();
        }
    }

    private void RebuildMusicLookup()
    {
        musicByScene.Clear();

        for (int i = 0; i < sceneMusic.Count; i++)
        {
            SceneMusic entry = sceneMusic[i];
            if (entry == null) continue;
            if (string.IsNullOrWhiteSpace(entry.sceneName)) continue;
            if (entry.musicClip == null) continue;

            musicByScene[entry.sceneName.Trim()] = entry.musicClip;
        }
    }
    #endregion

    #region SCENE MUSIC
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!playMusicOnSceneLoad) return;

        RebuildMusicLookup();
        PlayMusicForScene(scene.name);
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip clip = GetMusicForScene(sceneName);
        if (clip != null)
            PlayMusic(clip);
    }

    public AudioClip GetMusicForScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return null;

        if (musicByScene.TryGetValue(sceneName, out AudioClip listClip) && listClip != null)
            return listClip;

        // Fallback so your current inspector assignments keep working.
        switch (sceneName)
        {
            case "MainMenu": return mainMenuMusic;
            case "Tutorial": return tutorialMusic;
            case "Level1": return level1Music;
            case "Level2": return level2Music;
            case "Level3": return level3Music;
            case "LevelScene": return levelSceneMusic;
            default: return null;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (!restartSameClip && musicSource.clip == clip && musicSource.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (fadeDuration <= 0f)
        {
            musicSource.clip = clip;
            musicSource.Play();
            ApplySourceVolumes();
            return;
        }

        fadeCoroutine = StartCoroutine(FadeMusicRoutine(clip));
    }

    private IEnumerator FadeMusicRoutine(AudioClip newClip)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        float targetVolume = GetFinalMusicVolume();
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        fadeCoroutine = null;
    }
    #endregion

    #region UI SOUNDS
    public void PlayClick()
    {
        PlaySFX(buttonClick);
    }

    public void PlayHover()
    {
        PlaySFX(buttonHover);
    }
    #endregion

    #region GAME SFX
    public void PlayBaguetteThrow() => PlaySFX(baguetteThrow);
    public void PlayEnemyDamage() => PlaySFX(enemyDamage);
    public void PlayEnemyHurt() => PlaySFX(enemyHurt);
    public void PlayPlayerFall() => PlaySFX(playerFall);
    public void PlayPlayerFallHit() => PlaySFX(playerFallHit);
    public void PlayFireHurt() => PlaySFX(fireHurt);
    public void PlayPlayerJump() => PlaySFX(playerJump);
    public void PlaySpikeHit() => PlaySFX(spikeHit);
    public void PlayPlayerHurt() => PlaySFX(playerHurt);

    public void PlayFireAmbient(float volume)
    {
        PlaySFX(fireLoop, volume);
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null || sfxSource == null) return;

        float finalVolume = GetFinalSFXVolume() * Mathf.Max(0f, volumeMultiplier);
        sfxSource.PlayOneShot(clip, finalVolume);
    }
    #endregion

    #region VOLUME CONTROL
    public void SetMasterVolume(float normalizedValue)
    {
        masterVolume = Mathf.Clamp01(normalizedValue);
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume * 100f);
        PlayerPrefs.Save();
        ApplySourceVolumes();
    }

    public void SetMusicVolumeOnly(float normalizedValue)
    {
        musicVolume = Mathf.Clamp01(normalizedValue);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume * 100f);
        PlayerPrefs.Save();
        ApplySourceVolumes();
    }

    public void SetSFXVolumeOnly(float normalizedValue)
    {
        sfxVolume = Mathf.Clamp01(normalizedValue);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume * 100f);
        PlayerPrefs.Save();
        ApplySourceVolumes();
    }

    public float GetMasterVolume01() => masterVolume;
    public float GetMusicVolume01() => musicVolume;
    public float GetSFXVolume01() => sfxVolume;

    private void SaveAllVolumes()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume * 100f);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume * 100f);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume * 100f);
        PlayerPrefs.Save();
    }

    private void ApplySourceVolumes()
    {
        if (musicSource != null)
            musicSource.volume = GetFinalMusicVolume();

        if (sfxSource != null)
            sfxSource.volume = GetFinalSFXVolume();
    }

    private float GetFinalMusicVolume()
    {
        return Mathf.Clamp01(masterVolume * musicVolume * musicOutputMultiplier);
    }

    private float GetFinalSFXVolume()
    {
        return Mathf.Clamp01(masterVolume * sfxVolume * sfxOutputMultiplier);
    }
    #endregion

    #region PAUSE MUSIC
    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }
    #endregion
}

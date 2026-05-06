using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    #region SINGLETON

    public static SoundManager Instance;

    #endregion

    #region UI SOUNDS

    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;

    #endregion

    #region MUSIC

    [Header("Music")]
    public AudioClip mainMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip tutorialMusic;
    public AudioClip levelSceneMusic;

    [Header("Music Settings")]
    public float musicVolume = 0.01f;
    public float sfxVolume = 0.1f;
    public float fadeDuration = 1f;

    #endregion

    #region PRIVATE VARIABLES

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Coroutine fadeCoroutine;

    #endregion

    #region AWAKE

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float savedSlider = PlayerPrefs.GetFloat("Audio", 50f);
        float normalized = savedSlider / 100f;

        musicVolume = normalized * 0.08f;
        sfxVolume = normalized * 1f;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #endregion

    #region SCENE MUSIC

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip newMusic = null;

        if (scene.name == "MainMenu")
            newMusic = mainMenuMusic;
        else if (scene.name == "Tutorial")
            newMusic = tutorialMusic;
        else if (scene.name == "Level1")
            newMusic = level1Music;
        else if (scene.name == "Level2")
            newMusic = level2Music;
        else if (scene.name == "Level3")
            newMusic = level3Music;
        else if (scene.name == "LevelScene")
            newMusic = levelSceneMusic;

        if (newMusic != null && newMusic != musicSource.clip)
        {
            PlayMusic(newMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMusic(clip));
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newClip)
    {
        float elapsed = 0f;
        float startVolume = musicSource.volume;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(
                startVolume,
                0f,
                elapsed / fadeDuration
            );

            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(
                0f,
                musicVolume,
                elapsed / fadeDuration
            );

            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    #endregion

    #region UI SOUNDS

    public void PlayClick()
    {
        if (buttonClick != null)
            sfxSource.PlayOneShot(buttonClick, sfxVolume);
    }

    public void PlayHover()
    {
        if (buttonHover != null)
            sfxSource.PlayOneShot(buttonHover, sfxVolume);
    }

    #endregion

    #region VOLUME CONTROL

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
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

    #region CLEANUP

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion
}
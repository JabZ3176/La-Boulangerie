using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // SINGLETON
    // ─────────────────────────────────────────────
    public static SoundManager Instance;

    // ─────────────────────────────────────────────
    // UI SOUNDS
    // ─────────────────────────────────────────────
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;

    // ─────────────────────────────────────────────
    // MUSIC
    // ─────────────────────────────────────────────
    [Header("Music")]
    public AudioClip mainMenuMusic;     // drag main menu music here
    public AudioClip level1Music;       // drag level 1 music here
    public AudioClip level2Music;       // drag level 2 music here
    public AudioClip level3Music;       // drag level 3 music here
    public AudioClip tutorialMusic;     // drag tutorial music here

    [Header("Music Settings")]
    public float musicVolume = 0.5f;    // overall music volume
    public float sfxVolume = 1f;        // overall sfx volume
    public float fadeDuration = 1f;     // how long music fades in and out

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private AudioSource musicSource;    // plays background music
    private AudioSource sfxSource;      // plays sound effects
    private Coroutine fadeCoroutine;    // reference to the fade coroutine

    // ─────────────────────────────────────────────
    // AWAKE
    // ─────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // create two separate audio sources
        // one for music, one for sfx
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;            // music always loops
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;

        // listen for scene changes to swap music
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ─────────────────────────────────────────────
    // ON SCENE LOADED — swaps music when scene changes
    // ─────────────────────────────────────────────
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // pick the right music for each scene
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

        // only swap if the music is different from what is playing
        if (newMusic != null && newMusic != musicSource.clip)
        {
            PlayMusic(newMusic);
        }
    }

    // ─────────────────────────────────────────────
    // PLAY MUSIC — fades out old music and fades in new
    // ─────────────────────────────────────────────
    public void PlayMusic(AudioClip clip)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMusic(clip));
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newClip)
    {
        // fade out current music
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // use unscaled so it works when paused
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        // swap the clip
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // fade in new music
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / fadeDuration);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    // ─────────────────────────────────────────────
    // UI SOUNDS
    // ─────────────────────────────────────────────
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

    // ─────────────────────────────────────────────
    // VOLUME CONTROL — call these from a settings menu
    // ─────────────────────────────────────────────
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }

    // ─────────────────────────────────────────────
    // PAUSE AND RESUME MUSIC — call from PauseMenu.cs
    // ─────────────────────────────────────────────
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    // ─────────────────────────────────────────────
    // CLEANUP
    // ─────────────────────────────────────────────
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
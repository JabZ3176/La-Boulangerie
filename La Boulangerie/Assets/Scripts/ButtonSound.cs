using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("Sound")]
    public AudioClip clickSound;        // drag your click sound here
    public float volume = 1f;           // volume of the click

    private AudioSource audioSource;    // the audio source that plays the sound

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        // get or add an audio source on this object
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // make sure it doesnt play on awake
        audioSource.playOnAwake = false;

        // find the button and hook up the click sound
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    // ─────────────────────────────────────────────
    // PLAY CLICK SOUND
    // ─────────────────────────────────────────────
    private void PlayClickSound()
{
    if (SoundManager.Instance != null)
        SoundManager.Instance.PlayClick();
    else if (clickSound != null)
        audioSource.PlayOneShot(clickSound, volume); // fallback
}
}
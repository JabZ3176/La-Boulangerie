using UnityEngine;

public class Item : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────
    [Header("Ingredient Type")]
    public string itemType; // set to "Flour", "Milk" or "Butter"

    [Header("Sound")]
    public AudioClip pickupSound;   // drag your pickup sound here
    public float volume = 1f;

    // ─────────────────────────────────────────────
    // TRIGGER
    // ─────────────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // play sound before destroying
            PlayPickupSound();

            GameManager.Instance.CollectItem(itemType);
            Destroy(gameObject);
        }
    }

    // ─────────────────────────────────────────────
    // PLAY SOUND
    // ─────────────────────────────────────────────
    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            // use AudioSource.PlayClipAtPoint so sound plays
            // even after the object is destroyed
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
    }
}
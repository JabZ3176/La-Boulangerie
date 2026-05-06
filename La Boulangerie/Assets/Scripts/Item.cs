using UnityEngine;

public class Item : MonoBehaviour
{
    #region SETTINGS
    [Header("Ingredient Type")]
    public string itemType;

    [Header("Sound")]
    public AudioClip pickupSound;
    public float volume = 1f;
    #endregion

    #region TRIGGER
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayPickupSound();

            GameManager.Instance.CollectItem(itemType);
            Destroy(gameObject);
        }
    }
    #endregion

    #region SOUND
    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
    }
    #endregion
}

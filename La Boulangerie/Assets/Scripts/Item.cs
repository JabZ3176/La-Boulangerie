using UnityEngine;

public class Item : MonoBehaviour
{
    #region SETTINGS
    [Header("Ingredient Type")]
    public string itemType;

    [Header("Unique ID")]
    public string itemID;   // set a unique ID for each item in the Inspector
                            // e.g. "Level1_Flour_1", "Level1_Flour_2" etc

    [Header("Sound")]
    public AudioClip pickupSound;
    public float volume = 1f;
    #endregion

    #region START
    void Start()
    {
        // check if this item has already been collected in a previous run
        if (IsAlreadyCollected())
        {
            // already collected — hide it permanently
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region TRIGGER
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (IsAlreadyCollected()) return;

        PlayPickupSound();

        // mark this item as permanently collected before telling the GameManager.
        // This makes the saved item state and the UI/door progress agree if the level reloads.
        if (!string.IsNullOrEmpty(itemID))
        {
            PlayerPrefs.SetInt("Collected_" + itemID, 1);
            PlayerPrefs.Save();
        }

        if (GameManager.Instance != null)
            GameManager.Instance.CollectItem(itemType);

        Destroy(gameObject);
    }
    #endregion

    #region SAVE CHECK
    private bool IsAlreadyCollected()
    {
        if (string.IsNullOrEmpty(itemID)) return false;
        return PlayerPrefs.GetInt("Collected_" + itemID, 0) == 1;
    }
    #endregion

    #region SOUND
    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(pickupSound, volume);
            else
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
    }
    #endregion
}

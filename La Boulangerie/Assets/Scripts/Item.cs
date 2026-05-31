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
        if (PlayerPrefs.GetInt("Collected_" + itemID, 0) == 1)
        {
            // already collected — hide it permanently
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region TRIGGER
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayPickupSound();

            // mark this item as permanently collected
            PlayerPrefs.SetInt("Collected_" + itemID, 1);
            PlayerPrefs.Save();

            GameManager.Instance.CollectItem(itemType);
            Destroy(gameObject);
        }
    }
    #endregion

    #region SOUND
    private void PlayPickupSound()
    {
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
    }
    #endregion
}
using UnityEngine;

public class BaguettePickup : MonoBehaviour
{
    #region SETTINGS
    [Header("Respawn Settings")]
    public float respawnTime = 30f;

    [Header("Sound")]
    public AudioClip pickupSound;
    public float volume = 1f;
    #endregion

    #region PRIVATE VARIABLES
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool isCollected = false;
    private float respawnTimer = 0f;
    #endregion

    #region START
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    #endregion

    #region UPDATE
    void Update()
    {
        if (!isCollected) return;

        respawnTimer += Time.deltaTime;

        if (respawnTimer >= respawnTime)
        {
            Respawn();
        }
    }
    #endregion

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (player.AddBaguette())
                {
                    PlayPickupSound();
                    Collect();
                }
            }
        }
    }
    #endregion

    #region SOUND PLAYBACK
    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
    }
    #endregion

    #region PICKUP STATE
    private void Collect()
    {
        isCollected = true;
        respawnTimer = 0f;
        spriteRenderer.enabled = false;
        col.enabled = false;
    }

    private void Respawn()
    {
        isCollected = false;
        respawnTimer = 0f;
        spriteRenderer.enabled = true;
        col.enabled = true;
    }
    #endregion
}

using UnityEngine;

public class BaguettePickup : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────
    [Header("Respawn Settings")]
    public float respawnTime = 30f;

    [Header("Sound")]
    public AudioClip pickupSound;   // drag your pickup sound here
    public float volume = 1f;

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool isCollected = false;
    private float respawnTimer = 0f;

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // ─────────────────────────────────────────────
    // UPDATE
    // ─────────────────────────────────────────────
    void Update()
    {
        if (!isCollected) return;

        respawnTimer += Time.deltaTime;

        if (respawnTimer >= respawnTime)
        {
            Respawn();
        }
    }

    // ─────────────────────────────────────────────
    // TRIGGER
    // ─────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (player.AddBaguette())
                {
                    // play sound on pickup
                    PlayPickupSound();
                    Collect();
                }
            }
        }
    }

    // ─────────────────────────────────────────────
    // PLAY SOUND
    // ─────────────────────────────────────────────
    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
    }

    // ─────────────────────────────────────────────
    // COLLECT
    // ─────────────────────────────────────────────
    private void Collect()
    {
        isCollected = true;
        respawnTimer = 0f;
        spriteRenderer.enabled = false;
        col.enabled = false;
    }

    // ─────────────────────────────────────────────
    // RESPAWN
    // ─────────────────────────────────────────────
    private void Respawn()
    {
        isCollected = false;
        respawnTimer = 0f;
        spriteRenderer.enabled = true;
        col.enabled = true;
    }
}
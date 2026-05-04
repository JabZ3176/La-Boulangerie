using UnityEngine;

public class BaguettePickup : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────
    [Header("Respawn Settings")]
    public float respawnTime = 30f;     // how long before the baguette reappears

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private SpriteRenderer spriteRenderer;  // reference to the sprite renderer
    private Collider2D col;                 // reference to the collider
    private bool isCollected = false;       // tracks if the baguette has been picked up
    private float respawnTimer = 0f;        // counts up to respawnTime

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // ─────────────────────────────────────────────
    // UPDATE — counts up the respawn timer
    // ─────────────────────────────────────────────
    void Update()
    {
        // only count if the baguette has been collected
        if (!isCollected) return;

        respawnTimer += Time.deltaTime;

        // once the timer reaches respawnTime bring it back
        if (respawnTimer >= respawnTime)
        {
            Respawn();
        }
    }

    // ─────────────────────────────────────────────
    // TRIGGER — fires when player touches the pickup
    // ─────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // only pick up if the player has room for more baguettes
                if (player.AddBaguette())
                {
                    Collect();
                }
            }
        }
    }

    // ─────────────────────────────────────────────
    // COLLECT — hides the baguette and starts timer
    // ─────────────────────────────────────────────
    private void Collect()
    {
        isCollected = true;
        respawnTimer = 0f;

        // hide the baguette visually but keep the object alive
        spriteRenderer.enabled = false;
        col.enabled = false;        // disable collider so player cant pick it up while hidden
    }

    // ─────────────────────────────────────────────
    // RESPAWN — makes the baguette reappear
    // ─────────────────────────────────────────────
    private void Respawn()
    {
        isCollected = false;
        respawnTimer = 0f;

        // show the baguette again
        spriteRenderer.enabled = true;
        col.enabled = true;         // re-enable collider so player can pick it up again

        Debug.Log("Baguette respawned!");
    }
}
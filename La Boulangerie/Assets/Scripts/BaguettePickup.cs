using UnityEngine;

public class BaguettePickup : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // TRIGGER — fires when player touches the pickup
    // ─────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // only pick up if the player has room for more baguettes
                if (player.AddBaguette())
                {
                    Destroy(gameObject); // remove the pickup from the scene
                }
            }
        }
    }
}
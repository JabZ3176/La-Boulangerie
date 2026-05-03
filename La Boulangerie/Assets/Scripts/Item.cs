using UnityEngine;

public class Item : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────
    [Header("Ingredient Type")]
    public string itemType; // set this to exactly "Flour", "Milk" or "Butter" in Inspector

    // ─────────────────────────────────────────────
    // TRIGGER — fires when player touches the item
    // ─────────────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // tell the GameManager which type was collected
            GameManager.Instance.CollectItem(itemType);
            Destroy(gameObject);
        }
    }
}
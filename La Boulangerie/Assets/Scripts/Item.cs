using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName; // set this in the Inspector for each item

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectItem(itemName);
            Destroy(gameObject); // make the item disappear
        }
    }
}
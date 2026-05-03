using UnityEngine;

public class BaguetteProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float lifetime = 3f;
    public int damage = 2;

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool hasHit = false; // guard so it can only hit once

    public void Launch(Vector2 throwDirection, bool isJumping)
    {
        rb = GetComponent<Rigidbody2D>();

        if (isJumping)
        {
            // full gravity for jump throw so it arcs nicely
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(
                throwDirection.x * speed,
                speed * 0.8f
            );
        }
        else
        {
            // very low gravity for ground throw so it flies straight
            rb.gravityScale = 0.1f;
            rb.linearVelocity = new Vector2(
                throwDirection.x * speed,
                0f
            );
        }

        if (throwDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // stop if already hit something
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            // only process the trigger collider not the physical one
            if (!other.isTrigger) return;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                hasHit = true; // mark as hit so it cant hit again
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
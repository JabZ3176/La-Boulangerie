using UnityEngine;

public class BaguetteProjectile : MonoBehaviour
{
    #region SETTINGS
    [Header("Settings")]
    public float speed = 10f;
    public float lifetime = 3f;
    public int damage = 2;
    #endregion

    #region PRIVATE VARIABLES
    private Rigidbody2D rb;
    private bool hasHit = false;
    #endregion

    #region AWAKE
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region LAUNCH
    public void Launch(Vector2 throwDirection, bool isJumping)
    {
        if (isJumping)
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(
                throwDirection.x * speed,
                speed * 0.8f
            );
        }
        else
        {
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
    #endregion

    #region TRIGGERS
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            if (!other.isTrigger) return;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                hasHit = true;
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
    #endregion
}
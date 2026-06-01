using UnityEngine;

public class BaguetteProjectile : MonoBehaviour
{
    #region SETTINGS
    [Header("Settings")]
    public float speed = 10f;
    public float lifetime = 3f;
    public int baseDamage = 2;
    public int damage = 2;
    #endregion

    #region PRIVATE VARIABLES
    private Rigidbody2D rb;
    private bool hasHit = false;
    #endregion

    #region AWAKE
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        int savedDamageLevel = PlayerPrefs.GetInt("Upgrade_BaguetteDamage", 0);
        int singletonDamageLevel = PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.baguetteDamageLevel : 0;
        int bonusDamage = Mathf.Clamp(Mathf.Max(savedDamageLevel, singletonDamageLevel), 0, 2);

        damage = baseDamage + bonusDamage;
    }
    #endregion

    #region LAUNCH
    public void Launch(Vector2 throwDirection, bool isJumping)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

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
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (throwDirection.x > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);

        Destroy(gameObject, lifetime);
    }
    #endregion

    #region HIT DETECTION
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        TryHitEnemy(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (TryHitEnemy(collision.collider)) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

    private bool TryHitEnemy(Collider2D other)
    {
        if (other == null || hasHit) return false;

        Enemy2D enemy2D = other.GetComponentInParent<Enemy2D>();
        if (enemy2D != null)
        {
            hasHit = true;
            enemy2D.TakeDamage(damage, transform.position);
            Destroy(gameObject);
            return true;
        }

        bool isEnemyTagged = other.CompareTag("Enemy") ||
                             other.transform.root.CompareTag("Enemy") ||
                             (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Enemy"));

        if (!isEnemyTagged) return false;

        hasHit = true;

        GameObject receiver = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.transform.root.gameObject;

        receiver.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
        return true;
    }
    #endregion
}

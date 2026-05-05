using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // PATROL
    // ─────────────────────────────────────────────
    [Header("Patrol")]
    public float speed = 2f;        // how fast the enemy moves between patrol points
    public Transform[] points;      // the two or more patrol points set in the Inspector

    // ─────────────────────────────────────────────
    // STATS
    // ─────────────────────────────────────────────
    [Header("Stats")]
    public int maxHealth = 3;       // how many hits the enemy can take before dying
    private int currentHealth;      // tracks current health during gameplay
    private bool isDead = false;    // stops death from firing more than once

    // ─────────────────────────────────────────────
    // STUN
    // ─────────────────────────────────────────────
    [Header("Stun")]
    public float stunDuration = 2f; // how long the enemy is stunned after being slammed
    private bool isStunned = false; // tracks whether the enemy is currently stunned

    // ─────────────────────────────────────────────
    // DAMAGE
    // ─────────────────────────────────────────────
    [Header("Damage")]
    public int damageAmount = 1;        // how much damage the enemy deals to the player
    public float damageCooldown = 1f;   // seconds between each hit
    private float lastDamageTime = -1f; // tracks when the enemy last dealt damage

    // ─────────────────────────────────────────────
    // ATTACK
    // ─────────────────────────────────────────────
    [Header("Attack")]
    public float attackRange = 1.5f;    // how close the player needs to be to trigger attack
    public Transform playerTransform;   // reference to the player position

    [Header("References")]
    public Animator enemyAnimator;  // drag Actual Enemy here in the Inspector

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private int i;                          // current patrol point index
    private SpriteRenderer spriteRenderer;  // the enemy's sprite renderer
    private Animator animator;              // the enemy's animator
    private Color originalColor;            // stores the original sprite color
    private bool isAttacking = false;       // tracks if enemy is currently attacking

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    // ─────────────────────────────────────────────
    // UPDATE — runs every frame
    // ─────────────────────────────────────────────
    void Update()
    {
        if (isDead || isStunned) return;

        CheckAttackRange();

        // only patrol when not attacking
        if (!isAttacking)
        {
            Patrol();
        }
    }

    // ─────────────────────────────────────────────
    // CHECK ATTACK RANGE
    // ─────────────────────────────────────────────
    private void CheckAttackRange()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is NULL!");
            return;
        }

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            playerTransform.position
        );

        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetBool("IsAttacking", true);
            }
        }
        else
        {
            if (isAttacking)
            {
                isAttacking = false;
                animator.SetBool("IsAttacking", false);
            }
        }
    }

    // ─────────────────────────────────────────────
    // PATROL
    // ─────────────────────────────────────────────
    private void Patrol()
    {
        if (points.Length == 0) return;

        if (Vector2.Distance(transform.position, points[i].position) < 0.25f)
        {
            i++;
            if (i == points.Length) i = 0;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            points[i].position,
            speed * Time.deltaTime
        );

        spriteRenderer.flipX = (transform.position.x - points[i].position.x) > 0f;
    }

    // ─────────────────────────────────────────────
    // PLAYER CONTACT — deals damage when touching player
    // ─────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other.gameObject);
        }
    }

    private void DealDamageToPlayer(GameObject playerObject)
    {
        if (isStunned) return;

        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;

        Player player = playerObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }

    // ─────────────────────────────────────────────
    // STUN — called from Player.cs after a successful slam
    // ─────────────────────────────────────────────
    public void Stun()
    {
        if (isDead) return;
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        isAttacking = false;

        // stop the attack animation while stunned
        if (animator != null)
            animator.SetBool("IsAttacking", false);

        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        spriteRenderer.color = originalColor;
    }

    // ─────────────────────────────────────────────
    // TAKE DAMAGE — called from Player.cs after a slam
    // ─────────────────────────────────────────────
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = isStunned ? Color.yellow : originalColor;
    }

    // ─────────────────────────────────────────────
    // DEATH
    // ─────────────────────────────────────────────
    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        Destroy(gameObject, 0.2f);
    }
}
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region PATROL
    [Header("Patrol")]
    public float speed = 2f;
    public Transform[] points;
    #endregion

    #region STATS
    [Header("Stats")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    #endregion

    #region STUN
    [Header("Stun")]
    public float stunDuration = 2f;
    private bool isStunned = false;
    #endregion

    #region DAMAGE
    [Header("Damage")]
    public int damageAmount = 1;
    public float damageCooldown = 1f;
    private float lastDamageTime = -1f;
    #endregion

    #region ATTACK
    [Header("Attack")]
    public float attackRange = 1.5f;
    public Transform playerTransform;
    #endregion

    #region REFERENCES
    [Header("References")]
    public Animator enemyAnimator;
    #endregion

    #region PRIVATE VARIABLES
    private int i;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private bool isAttacking = false;
    #endregion

    #region START
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
    #endregion

    #region UPDATE
    void Update()
    {
        if (isDead || isStunned) return;

        CheckAttackRange();

        if (!isAttacking)
        {
            Patrol();
        }
    }
    #endregion

    #region ATTACK RANGE
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
    #endregion

    #region PATROL MOVEMENT
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
    #endregion

    #region PLAYER CONTACT
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
    #endregion

    #region STUN CONTROL
    public void Stun()
    {
        if (isDead) return;
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        isAttacking = false;

        if (animator != null)
            animator.SetBool("IsAttacking", false);

        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        spriteRenderer.color = originalColor;
    }
    #endregion

    #region DAMAGE CONTROL
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
    #endregion

    #region DEATH
    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        Destroy(gameObject, 0.2f);
    }
    #endregion
}

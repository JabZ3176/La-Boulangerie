using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum EnemyState
    {
        Patrolling,
        AttackWindup,
        AttackActive,
        AttackRecovery,
        Stunned,
        Dead
    }

    #region PATROL
    [Header("Patrol")]
    public float speed = 2f;
    public Transform[] points;
    public float pointReachDistance = 0.25f;
    #endregion

    #region STATS
    [Header("Stats")]
    public int maxHealth = 3;
    private int currentHealth;
    #endregion

    #region STUN
    [Header("Stun")]
    public float stunDuration = 2f;
    #endregion

    #region DAMAGE
    [Header("Damage")]
    public int damageAmount = 1;
    #endregion

    #region ATTACK
    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackWindupTime = 0.25f;
    public float attackActiveTime = 0.2f;
    public float attackRecoveryTime = 0.4f;
    public Transform playerTransform;
    #endregion

    #region REFERENCES
    [Header("References")]
    public Animator enemyAnimator;
    public PlayerDamageParticles damageParticles;
    #endregion

    #region PRIVATE VARIABLES
    private EnemyState currentState = EnemyState.Patrolling;

    private int currentPointIndex = 0;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;

    private Coroutine currentStateCoroutine;
    private bool hasDamagedThisAttack = false;
    private Rigidbody2D rb;
    #endregion

    #region UNITY
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = enemyAnimator != null ? enemyAnimator : GetComponentInChildren<Animator>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        ChangeState(EnemyState.Patrolling);
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead) return;
        if (currentState == EnemyState.Stunned) return;

        switch (currentState)
        {
            case EnemyState.Patrolling:
                HandlePatrolState();
                break;

            case EnemyState.AttackWindup:
                StopEnemyMovement();
                FacePlayer();
                break;

            case EnemyState.AttackActive:
                StopEnemyMovement();
                FacePlayer();
                break;

            case EnemyState.AttackRecovery:
                StopEnemyMovement();
                FacePlayer();
                break;
        }
    }
    #endregion

    #region STATE CONTROL
    private void ChangeState(EnemyState newState)
    {
        if (currentStateCoroutine != null)
        {
            StopCoroutine(currentStateCoroutine);
            currentStateCoroutine = null;
        }

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Patrolling:
                SetWalking(true);
                SetAttacking(false);
                break;

            case EnemyState.AttackWindup:
                currentStateCoroutine = StartCoroutine(AttackRoutine());
                break;

            case EnemyState.Stunned:
                currentStateCoroutine = StartCoroutine(StunRoutine());
                break;

            case EnemyState.Dead:
                HandleDeathState();
                break;
        }
    }
    #endregion

    #region PATROL
    private void HandlePatrolState()
    {
        if (PlayerInAttackRange())
        {
            ChangeState(EnemyState.AttackWindup);
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        if (points == null || points.Length == 0)
        {
            StopEnemyMovement();
            SetWalking(false);
            return;
        }

        SetWalking(true);
        SetAttacking(false);

        Transform targetPoint = points[currentPointIndex];

        if (targetPoint == null)
            return;

        Vector2 direction = ((Vector2)targetPoint.position - (Vector2)transform.position).normalized;
        Vector2 newPosition = Vector2.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        if (rb != null)
            rb.MovePosition(newPosition);
        else
            transform.position = newPosition;

        if (direction.x > 0.01f)
            SetFacingRight(true);
        else if (direction.x < -0.01f)
            SetFacingRight(false);

        float distanceToPoint = Vector2.Distance(transform.position, targetPoint.position);

        if (distanceToPoint <= pointReachDistance)
        {
            currentPointIndex++;

            if (currentPointIndex >= points.Length)
                currentPointIndex = 0;
        }
    }

    private void StopEnemyMovement()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
    #endregion

    #region ATTACK
    private IEnumerator AttackRoutine()
    {
        hasDamagedThisAttack = false;

        StopEnemyMovement();
        SetWalking(false);
        SetAttacking(true);
        FacePlayer();

        currentState = EnemyState.AttackWindup;
        yield return new WaitForSeconds(attackWindupTime);

        currentState = EnemyState.AttackActive;
        yield return new WaitForSeconds(attackActiveTime);

        currentState = EnemyState.AttackRecovery;
        SetAttacking(false);
        yield return new WaitForSeconds(attackRecoveryTime);

        if (currentState == EnemyState.Stunned || currentState == EnemyState.Dead)
            yield break;

        if (PlayerInAttackRange())
            ChangeState(EnemyState.AttackWindup);
        else
            ChangeState(EnemyState.Patrolling);
    }

    private bool PlayerInAttackRange()
    {
        if (playerTransform == null)
            return false;

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            playerTransform.position
        );

        return distanceToPlayer <= attackRange;
    }

    private void TryDamagePlayer(GameObject playerObject)
    {
        if (currentState != EnemyState.AttackActive) return;
        if (hasDamagedThisAttack) return;

        Player player = playerObject.GetComponent<Player>();
        if (player == null) return;

        hasDamagedThisAttack = true;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayEnemyDamage();

        player.TakeDamage(true);
    }
    #endregion

    #region COLLISIONS
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TryDamagePlayer(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TryDamagePlayer(other.gameObject);
    }
    #endregion

    #region STUN
    public void Stun()
    {
        if (currentState == EnemyState.Dead) return;

        ChangeState(EnemyState.Stunned);
    }

    private IEnumerator StunRoutine()
    {
        StopEnemyMovement();
        SetWalking(false);
        SetAttacking(false);
        hasDamagedThisAttack = false;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(stunDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        ChangeState(EnemyState.Patrolling);
    }
    #endregion

    #region DAMAGE
    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead) return;

        currentHealth -= damage;

        if (damageParticles != null)
            damageParticles.EmitDamageParticles();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayEnemyHurt();

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            ChangeState(EnemyState.Dead);
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (currentState == EnemyState.Stunned)
            spriteRenderer.color = Color.yellow;
        else if (currentState != EnemyState.Dead)
            spriteRenderer.color = originalColor;
    }
    #endregion

    #region DEATH
    private void HandleDeathState()
    {
        StopEnemyMovement();
        SetWalking(false);
        SetAttacking(false);

        if (currentStateCoroutine != null)
        {
            StopCoroutine(currentStateCoroutine);
            currentStateCoroutine = null;
        }

        Destroy(gameObject, 0.2f);
    }
    #endregion

    #region FACING
    private void FacePlayer()
    {
        if (playerTransform == null) return;

        bool playerIsRight = playerTransform.position.x > transform.position.x;
        SetFacingRight(playerIsRight);
    }

    private void SetFacingRight(bool facingRight)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.flipX = !facingRight;
    }
    #endregion

    #region ANIMATION
    private void SetWalking(bool walking)
    {
        if (animator != null)
            animator.SetBool("IsWalking", walking);
    }

    private void SetAttacking(bool attacking)
    {
        if (animator != null)
            animator.SetBool("IsAttacking", attacking);
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (points != null)
        {
            Gizmos.color = Color.cyan;

            for (int index = 0; index < points.Length; index++)
            {
                if (points[index] == null) continue;

                Gizmos.DrawWireSphere(points[index].position, 0.15f);

                int nextIndex = index + 1;
                if (nextIndex >= points.Length)
                    nextIndex = 0;

                if (points[nextIndex] != null)
                    Gizmos.DrawLine(points[index].position, points[nextIndex].position);
            }
        }
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy2D : MonoBehaviour
{
    private enum EnemyState
    {
        Patrolling,
        Waiting,
        Stunned,
        Dead
    }

    private enum FacingMode
    {
        FlipVisualRootScale,
        FlipSpriteRendererOnly
    }

    [Header("References")]
    [Tooltip("The parent object containing the sprite, dust particles, and blood particles. This is what flips left/right.")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyVFX2D vfx;

    [Header("Footsteps")]
    [Tooltip("Drag one or more enemy footstep clips here, just like the player footstepSounds array.")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepVolume = 0.2f;
    [Tooltip("ON plays footsteps from this script while the enemy patrols. OFF lets you use Animation Events that call PlayFootstep instead.")]
    [SerializeField] private bool useAutomaticFootsteps = true;
    [SerializeField] private float automaticFootstepInterval = 0.32f;
    [Tooltip("Recommended ON. Prevents footstep sounds while waiting, stunned, dead, or playing a touch attack animation.")]
    [SerializeField] private bool onlyPlayFootstepsWhilePatrolling = true;

    [Header("Patrol Between 2 Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [Tooltip("Recommended ON. This keeps child Point A / Point B objects from moving with the enemy during play.")]
    [SerializeField] private bool lockPointPositionsOnStart = true;
    [SerializeField] private float walkSpeed = 1.7f;
    [SerializeField] private float pointReachDistance = 0.08f;
    [SerializeField] private float waitAtPointTime = 0.2f;

    [Header("Solid Blocking")]
    [Tooltip("ON makes the enemy kinematic so the player cannot shove it around.")]
    [SerializeField] private bool forceKinematicBody = true;
    [Tooltip("The main body collider should be solid, not a trigger, so the player must go around or kill the enemy.")]
    [SerializeField] private bool forceBodyColliderSolid = true;

    [Header("Player Contact Damage")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool damagePlayerOnSideTouch = true;
    [SerializeField] private float playerDamageCooldown = 0.75f;
    [SerializeField] private float touchAttackAnimationTime = 0.22f;

    [Header("Stomp")]
    [SerializeField] private int stompDamage = 1;
    [SerializeField] private float stompStunTime = 0.45f;
    [SerializeField] private float stompCooldown = 0.12f;
    [Tooltip("Enemy only counts as stomped when the player is above this height relative to the enemy center.")]
    [SerializeField] private float minimumPlayerHeightAboveEnemy = 0.15f;
    [Tooltip("Enemy only counts as stomped when the player is moving down at least this fast. Slamming easily meets this.")]
    [SerializeField] private float minimumDownVelocityForStomp = 1.0f;
    [Tooltip("Used only if the Enemy2D detects the stomp itself before Player.cs does.")]
    [SerializeField] private float fallbackStompBounceVelocity = 10f;
    [Tooltip("ON means stomps only count while the player is actively holding S or Down Arrow. Normal jumps can land on the enemy without stomping.")]
    [SerializeField] private bool requireStompInputHeld = true;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float hurtInvulnerabilityTime = 0.08f;
    [SerializeField] private float deathDestroyDelay = 0.15f;

    [Header("Visual Juice")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private Color stompFlashColor = Color.yellow;
    [SerializeField] private float flashTime = 0.08f;
    [SerializeField] private float dustStepInterval = 0.12f;
    [SerializeField] private FacingMode facingMode = FacingMode.FlipVisualRootScale;

    [Header("Animator Parameters")]
    [Tooltip("Your provided controller already has IsAttacking. Add IsMoving or IsWalking if you want bool-based walk transitions.")]
    [SerializeField] private bool useAnimatorParameters = true;
    [Tooltip("This lets the script play your Idle/Walk/Attack state names directly if those states exist in the controller.")]
    [SerializeField] private bool useDirectStateNames = true;
    [SerializeField] private string idleStateName = "Enemy_Pin_Idle";
    [SerializeField] private string walkStateName = "Enemy_Pin_Walk";
    [SerializeField] private string attackStateName = "Enemy_Pin_Attack";

    [Header("Events")]
    public UnityEvent onStomped;
    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private readonly HashSet<int> animatorParameters = new HashSet<int>();

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private SpriteRenderer[] flashRenderers;
    private Color[] originalRendererColors;

    private EnemyState state = EnemyState.Patrolling;
    private Vector2 savedPointA;
    private Vector2 savedPointB;
    private bool goingToB = true;
    private int currentHealth;
    private float waitTimer;
    private float invulnerabilityTimer;
    private float lastPlayerDamageTime = -999f;
    private float lastStompTime = -999f;
    private float dustTimer;
    private float footstepTimer;
    private float facingSign = 1f;
    private bool attackAnimationActive;
    private Vector3 visualRootStartScale = Vector3.one;
    private Coroutine flashRoutine;
    private Coroutine attackVisualRoutine;
    private string currentDirectState = string.Empty;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => state == EnemyState.Dead;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        vfx = GetComponentInChildren<EnemyVFX2D>();
        visualRoot = spriteRenderer != null ? spriteRenderer.transform.parent : transform;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        if (bodyCollider != null)
            bodyCollider.isTrigger = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (vfx == null) vfx = GetComponentInChildren<EnemyVFX2D>();
        if (visualRoot == null && spriteRenderer != null) visualRoot = spriteRenderer.transform.parent != null ? spriteRenderer.transform.parent : spriteRenderer.transform;
        if (visualRoot == null) visualRoot = transform;

        visualRootStartScale = visualRoot.localScale;

        if (forceKinematicBody)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (forceBodyColliderSolid && bodyCollider != null)
            bodyCollider.isTrigger = false;

        CacheFlashRenderers();
        CacheAnimatorParameters();
        SavePatrolPointPositions();
    }

    private void Start()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        state = EnemyState.Patrolling;
        UpdateAnimatorValues(false);
    }

    private void Update()
    {
        if (state == EnemyState.Dead) return;

        float dt = Time.deltaTime;

        if (invulnerabilityTimer > 0f)
            invulnerabilityTimer -= dt;

        if (state == EnemyState.Waiting)
        {
            waitTimer -= dt;
            if (waitTimer <= 0f)
                state = EnemyState.Patrolling;
        }
        else if (state == EnemyState.Stunned)
        {
            waitTimer -= dt;
            if (waitTimer <= 0f)
                state = EnemyState.Patrolling;
        }

        UpdateMoveDust(dt);
        UpdateFootsteps(dt);
        UpdateAnimatorValues(IsMoving());
    }

    private void FixedUpdate()
    {
        if (state == EnemyState.Dead) return;
        if (state != EnemyState.Patrolling) return;

        Vector2 target = GetCurrentTargetPoint();
        Vector2 current = rb.position;
        float deltaX = target.x - current.x;

        if (Mathf.Abs(deltaX) <= pointReachDistance)
        {
            rb.MovePosition(new Vector2(target.x, current.y));
            SwapTargetPoint();
            state = EnemyState.Waiting;
            waitTimer = waitAtPointTime;
            return;
        }

        float direction = Mathf.Sign(deltaX);
        FaceDirection(direction);

        Vector2 nextPosition = Vector2.MoveTowards(
            current,
            new Vector2(target.x, current.y),
            walkSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
    }

    private void SavePatrolPointPositions()
    {
        Vector2 fallbackLeft = (Vector2)transform.position + Vector2.left;
        Vector2 fallbackRight = (Vector2)transform.position + Vector2.right;

        savedPointA = pointA != null ? (Vector2)pointA.position : fallbackLeft;
        savedPointB = pointB != null ? (Vector2)pointB.position : fallbackRight;
    }

    private Vector2 GetCurrentTargetPoint()
    {
        if (!lockPointPositionsOnStart)
        {
            if (goingToB && pointB != null) return pointB.position;
            if (!goingToB && pointA != null) return pointA.position;
        }

        return goingToB ? savedPointB : savedPointA;
    }

    private void SwapTargetPoint()
    {
        goingToB = !goingToB;
    }

    private bool IsMoving()
    {
        return state == EnemyState.Patrolling && Vector2.Distance(rb.position, GetCurrentTargetPoint()) > pointReachDistance;
    }

    private void FaceDirection(float direction)
    {
        if (Mathf.Approximately(direction, 0f)) return;

        facingSign = direction > 0f ? 1f : -1f;

        if (facingMode == FacingMode.FlipVisualRootScale && visualRoot != null)
        {
            Vector3 scale = visualRootStartScale;
            scale.x = Mathf.Abs(visualRootStartScale.x) * facingSign;
            visualRoot.localScale = scale;

            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingSign < 0f;
        }
    }

    private void UpdateMoveDust(float dt)
    {
        if (vfx == null) return;
        if (state != EnemyState.Patrolling) return;
        if (!IsMoving()) return;

        dustTimer -= dt;
        if (dustTimer > 0f) return;

        vfx.EmitMoveDust(facingSign);
        dustTimer = dustStepInterval;
    }

    private void UpdateFootsteps(float dt)
    {
        if (!useAutomaticFootsteps) return;

        if (!CanPlayFootstepNow())
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= dt;
        if (footstepTimer > 0f) return;

        PlayFootstep();
        footstepTimer = Mathf.Max(0.05f, automaticFootstepInterval);
    }

    // You can call this from an Animation Event on Enemy_Pin_Walk, exactly like the player's PlayFootstep method.
    public void PlayFootstep()
    {
        if (!CanPlayFootstepNow()) return;
        if (footstepSounds == null || footstepSounds.Length == 0) return;

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        if (clip == null) return;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(clip, footstepVolume);
        else
            AudioSource.PlayClipAtPoint(clip, transform.position, footstepVolume);
    }

    private bool CanPlayFootstepNow()
    {
        if (state == EnemyState.Dead) return false;
        if (state == EnemyState.Stunned) return false;
        if (attackAnimationActive) return false;
        if (onlyPlayFootstepsWhilePatrolling && state != EnemyState.Patrolling) return false;

        return IsMoving();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerCollision(collision.collider, collision.rigidbody);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandlePlayerCollision(collision.collider, collision.rigidbody);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // This is here in case your enemy body is accidentally left as a trigger.
        HandlePlayerCollision(other, other.attachedRigidbody);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandlePlayerCollision(other, other.attachedRigidbody);
    }

    private void HandlePlayerCollision(Collider2D other, Rigidbody2D otherBody)
    {
        if (state == EnemyState.Dead) return;
        if (other == null) return;

        Player player = GetPlayerFromCollider(other);
        if (player == null) return;

        Rigidbody2D playerBody = otherBody != null ? otherBody : other.attachedRigidbody;

        bool playerIsOnTop = IsPlayerAboveEnemy(other);

        if (CanBeStompedBy(other, playerBody))
        {
            TryStomp(player, playerBody, fallbackStompBounceVelocity);
            return;
        }

        // Landing or standing on top of the enemy is allowed.
        // It only becomes a stomp when S or Down Arrow is being held.
        if (playerIsOnTop)
            return;

        if (damagePlayerOnSideTouch)
            DamagePlayer(player);
    }

    private Player GetPlayerFromCollider(Collider2D other)
    {
        if (other == null) return null;

        if (!string.IsNullOrEmpty(playerTag))
        {
            bool taggedAsPlayer = other.CompareTag(playerTag) ||
                                  other.transform.root.CompareTag(playerTag) ||
                                  (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(playerTag));

            if (!taggedAsPlayer) return null;
        }

        Player player = other.GetComponentInParent<Player>();
        if (player != null) return player;

        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.GetComponent<Player>();

        return null;
    }

    private bool CanBeStompedBy(Collider2D playerCollider, Rigidbody2D playerBody)
    {
        if (state == EnemyState.Dead) return false;
        if (Time.time - lastStompTime < stompCooldown) return false;
        if (playerCollider == null) return false;
        if (requireStompInputHeld && !IsStompInputHeld()) return false;

        bool playerIsMovingDown = true;
        if (playerBody != null)
            playerIsMovingDown = playerBody.linearVelocity.y <= -minimumDownVelocityForStomp;

        return IsPlayerAboveEnemy(playerCollider) && playerIsMovingDown;
    }

    private bool IsPlayerAboveEnemy(Collider2D playerCollider)
    {
        if (playerCollider == null) return false;

        float playerBottom = playerCollider.bounds.min.y;
        float enemyCenterY = bodyCollider != null ? bodyCollider.bounds.center.y : transform.position.y;

        return playerBottom >= enemyCenterY + minimumPlayerHeightAboveEnemy;
    }

    private bool IsStompInputHeld()
    {
        return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    }

    public bool TryStomp(Player player, Rigidbody2D playerBody, float bounceVelocity)
    {
        if (state == EnemyState.Dead) return false;
        if (Time.time - lastStompTime < stompCooldown) return false;
        if (requireStompInputHeld && !IsStompInputHeld()) return false;

        lastStompTime = Time.time;

        if (player != null)
            player.RegisterStomp();

        if (playerBody != null && bounceVelocity > 0f)
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, bounceVelocity);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayPlayerFallHit();

        onStomped?.Invoke();
        ApplyDamage(stompDamage, playerBody != null ? playerBody.position : (Vector2)transform.position + Vector2.up, stompFlashColor, true);

        if (state != EnemyState.Dead && stompStunTime > 0f)
            Stun(stompStunTime);

        return true;
    }

    private void DamagePlayer(Player player)
    {
        if (player == null) return;
        if (Time.time - lastPlayerDamageTime < playerDamageCooldown) return;

        lastPlayerDamageTime = Time.time;

        PlayTouchAttackAnimation();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayEnemyDamage();

        player.TakeDamage(true);
    }

    public void TakeDamage(int amount)
    {
        Vector2 source = (Vector2)transform.position - new Vector2(facingSign, 0f);
        TakeDamage(amount, source);
    }

    public void TakeDamage(int amount, Vector2 damageSourceWorldPosition)
    {
        ApplyDamage(amount, damageSourceWorldPosition, damageFlashColor, false);
    }

    private void ApplyDamage(int amount, Vector2 damageSourceWorldPosition, Color flashColor, bool ignoreInvulnerability)
    {
        if (state == EnemyState.Dead) return;
        if (!ignoreInvulnerability && invulnerabilityTimer > 0f) return;

        amount = Mathf.Max(1, amount);
        currentHealth -= amount;
        invulnerabilityTimer = hurtInvulnerabilityTime;

        if (vfx != null)
            vfx.EmitBloodParticles(damageSourceWorldPosition, transform.position);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayEnemyHurt();

        onDamaged?.Invoke();
        StartFlash(flashColor, flashTime);
        TriggerAnimator(HurtHash);

        if (currentHealth <= 0)
            Die();
    }

    public void Stun()
    {
        Stun(stompStunTime);
    }

    public void Stun(float duration)
    {
        if (state == EnemyState.Dead) return;

        state = EnemyState.Stunned;
        waitTimer = Mathf.Max(0.05f, duration);
        StartFlash(stompFlashColor, waitTimer);
    }

    private void Die()
    {
        if (state == EnemyState.Dead) return;

        state = EnemyState.Dead;
        onDeath?.Invoke();

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        TriggerAnimator(DieHash);
        SetAnimatorBool(IsMovingHash, false);
        SetAnimatorBool(IsWalkingHash, false);
        SetAnimatorBool(IsAttackingHash, false);

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Destroy(gameObject, deathDestroyDelay);
    }

    private void PlayTouchAttackAnimation()
    {
        if (attackVisualRoutine != null)
            StopCoroutine(attackVisualRoutine);

        attackVisualRoutine = StartCoroutine(TouchAttackAnimationRoutine());
    }

    private IEnumerator TouchAttackAnimationRoutine()
    {
        attackAnimationActive = true;
        SetAnimatorBool(IsAttackingHash, true);
        PlayDirectState(attackStateName, true);

        yield return new WaitForSeconds(touchAttackAnimationTime);

        SetAnimatorBool(IsAttackingHash, false);
        attackAnimationActive = false;
        attackVisualRoutine = null;
    }

    private void CacheFlashRenderers()
    {
        flashRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalRendererColors = new Color[flashRenderers.Length];

        for (int i = 0; i < flashRenderers.Length; i++)
            originalRendererColors[i] = flashRenderers[i].color;
    }

    private void StartFlash(Color color, float duration)
    {
        if (flashRenderers == null || flashRenderers.Length == 0) return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        for (int i = 0; i < flashRenderers.Length; i++)
        {
            if (flashRenderers[i] != null)
                flashRenderers[i].color = color;
        }

        yield return new WaitForSeconds(duration);

        if (state != EnemyState.Dead)
        {
            for (int i = 0; i < flashRenderers.Length; i++)
            {
                if (flashRenderers[i] != null)
                    flashRenderers[i].color = originalRendererColors[i];
            }
        }

        flashRoutine = null;
    }

    private void CacheAnimatorParameters()
    {
        animatorParameters.Clear();
        if (animator == null) return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
            animatorParameters.Add(parameter.nameHash);
    }

    private void UpdateAnimatorValues(bool moving)
    {
        if (animator == null) return;

        if (useAnimatorParameters)
        {
            SetAnimatorBool(IsMovingHash, moving);
            SetAnimatorBool(IsWalkingHash, moving);
            SetAnimatorFloat(SpeedHash, moving ? walkSpeed : 0f);
        }

        if (!attackAnimationActive && useDirectStateNames)
            PlayDirectState(moving ? walkStateName : idleStateName, false);
    }

    private void SetAnimatorBool(int hash, bool value)
    {
        if (animator != null && animatorParameters.Contains(hash))
            animator.SetBool(hash, value);
    }

    private void SetAnimatorFloat(int hash, float value)
    {
        if (animator != null && animatorParameters.Contains(hash))
            animator.SetFloat(hash, value);
    }

    private void TriggerAnimator(int hash)
    {
        if (animator != null && animatorParameters.Contains(hash))
            animator.SetTrigger(hash);
    }

    private void PlayDirectState(string stateName, bool forceRestart)
    {
        if (animator == null) return;
        if (string.IsNullOrEmpty(stateName)) return;
        if (!forceRestart && currentDirectState == stateName) return;

        int stateHash = Animator.StringToHash(stateName);
        if (!animator.HasState(0, stateHash)) return;

        animator.Play(stateHash, 0, forceRestart ? 0f : float.NegativeInfinity);
        currentDirectState = stateName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 a = pointA != null ? pointA.position : transform.position + Vector3.left;
        Vector3 b = pointB != null ? pointB.position : transform.position + Vector3.right;

        Gizmos.DrawWireSphere(a, 0.12f);
        Gizmos.DrawWireSphere(b, 0.12f);
        Gizmos.DrawLine(a, b);

        Gizmos.color = Color.yellow;
        Vector3 stompCenter = bodyCollider != null ? bodyCollider.bounds.center : transform.position;
        Gizmos.DrawLine(stompCenter + Vector3.left * 0.4f + Vector3.up * minimumPlayerHeightAboveEnemy,
                        stompCenter + Vector3.right * 0.4f + Vector3.up * minimumPlayerHeightAboveEnemy);
    }
}

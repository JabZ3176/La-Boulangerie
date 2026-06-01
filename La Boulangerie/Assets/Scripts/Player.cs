using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    #region STATS
    [Header("Stats")]
    public int health = 3;
    public int maxHealth = 3;       // add this
    private bool isDead = false;
    private bool isInvincible = false;
    #endregion

    #region MOVEMENT
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpForce = 10f;
    public float jumpCutMultiplier = 0.5f;
    #endregion

    #region FOOTSTEPS
    [Header("Footsteps")]
    public AudioClip[] footstepSounds;
    public float footstepVolume = 0.2f;
    #endregion

    #region SLAM
    [Header("Slam")]
    public float slamForce = 25f;
    public float slamBounceForce = 10f;
    public float slamRadius = 0.6f;
    public LayerMask enemyLayer;

    private bool isSlamming = false;
    private bool slamOnCooldown = false;
    private float slamCooldownTime = 0.3f;
    #endregion

    #region CROISSANT ENERGY
    [Header("Croissant Energy Meter")]
    public float maxCroissantEnergy = 100f;
    public float croissantDrainRate = 20f;
    public float croissantRechargeRate = 10f;
    public float croissantMinToSprint = 10f;
    #endregion

    #region BAGUETTE THROWING
    [Header("Baguette Throwing")]
    public GameObject baguettePrefab;
    public Transform throwPoint;
    public float throwCooldown = 0.5f;
    public int maxBaguettes = 3;
    public BaguetteUI baguetteUI;

    private int currentBaguettes = 0;
    private float lastThrowTime = -1f;
    #endregion

    #region STOMP BUFF
    [Header("Stomp Buff")]
    public int stompsRequired = 5;
    public float buffDuration = 15f;
    public float buffSpeedMultiplier = 1.2f;
    public float buffJumpMultiplier = 1.2f;
    public BuffDisplay buffDisplay;

    private int currentStompChain = 0;
    private bool isBuffActive = false;
    private float buffTimer = 0f;
    private Color goldenColor = new Color(1f, 0.84f, 0f, 1f);
    private Coroutine buffFlashCoroutine;
    #endregion

    #region GROUND DETECTION
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    #endregion

    #region UI
    [Header("UI")]
    public HeartHealthBar heartHealthBar;
    public CroissantMeter croissantMeter;
    #endregion

    #region REFERENCES
    [Header("References")]
    public DeathMenu deathMenu;
    public PlayerDamageParticles damageParticles;
    #endregion

    #region PRIVATE VARIABLES
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private float killHeight = -10f;

    private bool isGrounded;
    private bool isJumping;
    private bool isSprinting;
    private bool hasDoubleJump;
    private bool canDoubleJump;
    private float currentCroissantEnergy;
    private bool slamLanded = false;
    private float fallTimer = 0f;          // tracks how long the player has been falling
    private bool isFalling = false;        // tracks if the player is currently falling

    private float originalMoveSpeed;
    private float originalSprintSpeed;
    private float originalJumpForce;

    private float baseMoveSpeed;
    private float baseSprintSpeed;
    private float baseJumpForce;
    private float baseCroissantDrainRate;

    private float idleTimer = 0f;

    private const int MaxHealth = 3;
    private const string DamageTag = "Damage";
    private const string EnemyTag = "Enemy";
    private const string StartScene = "Level1";
    private const string DoubleJumpScene = "Level2";
    #endregion

    #region UPGRADE APPLICATION
    public void ApplySavedUpgrades(bool refillHealth)
    {
        int healthLevel = GetUpgradeLevel("Upgrade_Health", PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.healthLevel : 0, 0, 3);
        int baguetteLevel = GetUpgradeLevel("Upgrade_Baguette", PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.baguetteLevel : 0, 0, 3);
        int staminaLevel = GetUpgradeLevel("Upgrade_Stamina", PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.staminaLevel : 0, 0, 2);
        int movementLevel = GetUpgradeLevel("Upgrade_Movement", PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.movementLevel : 0, 0, 2);
        int jumpLevel = GetUpgradeLevel("Upgrade_Jump", PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.jumpLevel : 0, 0, 2);

        maxHealth = Mathf.Clamp(3 + healthLevel, 3, 6);

        if (refillHealth)
            health = maxHealth;
        else
            health = Mathf.Clamp(health, 0, maxHealth);

        maxBaguettes = Mathf.Clamp(3 + baguetteLevel, 3, 6);

        croissantDrainRate = Mathf.Max(5f, baseCroissantDrainRate - (staminaLevel * 5f));
        moveSpeed = baseMoveSpeed + (movementLevel * 1.5f);
        sprintSpeed = baseSprintSpeed + (movementLevel * 2f);
        jumpForce = baseJumpForce + (jumpLevel * 2f);
    }

    private int GetUpgradeLevel(string prefsKey, int singletonValue, int min, int max)
    {
        // Use the highest value from the singleton and PlayerPrefs.
        // This protects the player if the singleton was not present when the shop saved the upgrade,
        // or if a stale PlayerUpgrades object exists in the scene.
        int savedValue = PlayerPrefs.GetInt(prefsKey, 0);
        int value = PlayerUpgrades.Instance != null ? Mathf.Max(singletonValue, savedValue) : savedValue;
        return Mathf.Clamp(value, min, max);
    }

    private void RefreshBaguetteCapacity()
    {
        int baguetteLevel = GetUpgradeLevel("Upgrade_Baguette", PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.baguetteLevel : 0, 0, 3);
        maxBaguettes = Mathf.Clamp(3 + baguetteLevel, 3, 6);
        currentBaguettes = Mathf.Clamp(currentBaguettes, 0, maxBaguettes);
    }

    private void UpdateBaguetteUI()
    {
        RefreshBaguetteCapacity();

        if (baguetteUI != null)
            baguetteUI.UpdateSlots(currentBaguettes, maxBaguettes);
    }
    #endregion

    #region START
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        originalColor = spriteRenderer.color;
        currentCroissantEnergy = maxCroissantEnergy;

        // Save the inspector values as the true base stats before upgrades are applied.
        baseMoveSpeed = moveSpeed;
        baseSprintSpeed = sprintSpeed;
        baseJumpForce = jumpForce;
        baseCroissantDrainRate = croissantDrainRate;

        // Apply saved upgrades even if the PlayerUpgrades singleton is missing in this scene.
        ApplySavedUpgrades(true);

        // The stomp buff should return to the upgraded stats, not the old base stats.
        originalMoveSpeed = moveSpeed;
        originalSprintSpeed = sprintSpeed;
        originalJumpForce = jumpForce;

        string currentScene = SceneManager.GetActiveScene().name;
        canDoubleJump = (currentScene == DoubleJumpScene);

        if (currentScene == "Level1") killHeight = -20f;
        else if (currentScene == "Level2") killHeight = -20f;
        else if (currentScene == "Level3") killHeight = -30f;

        if (heartHealthBar != null)
            heartHealthBar.UpdateHearts(health, maxHealth);

        UpdateBaguetteUI();

        if (croissantMeter != null)
            croissantMeter.UpdateCroissants(currentCroissantEnergy, maxCroissantEnergy);
    }
    #endregion

    #region UPDATE
    void Update()
    {
        HandleSprint();
        HandleMovement();
        HandleJump();
        HandleSlam();
        HandleBaguetteThrow();
        HandleIdleTimer();
        HandleBuff();
        UpdateHealthUI();
        UpdateCroissantUI();

        if (transform.position.y < killHeight)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayPlayerFall();

            Die();
        }
    }
    #endregion

    #region FIXED UPDATE
    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (!wasGrounded && isGrounded)
        {
            hasDoubleJump = canDoubleJump;
        }

        // track falling time using velocity
        if (rb.linearVelocity.y < -0.5f)
        {
            fallTimer += Time.fixedDeltaTime;
            isFalling = true;
        }
        else if (rb.linearVelocity.y > 0.1f)
        {
            fallTimer = 0f;
            isFalling = false;
        }
    }
    #endregion

    #region SPRINT
    private void HandleSprint()
    {
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftHeld && currentCroissantEnergy >= croissantMinToSprint)
        {
            isSprinting = true;
            currentCroissantEnergy -= croissantDrainRate * Time.deltaTime;
            currentCroissantEnergy = Mathf.Max(currentCroissantEnergy, 0f);
        }
        else
        {
            isSprinting = false;
            currentCroissantEnergy += croissantRechargeRate * Time.deltaTime;
            currentCroissantEnergy = Mathf.Min(currentCroissantEnergy, maxCroissantEnergy);
        }
    }
    #endregion

    #region MOVEMENT
    private void HandleMovement()
    {
        if (isSlamming) return;

        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // instantly stop horizontal movement when no input is pressed
        // this removes the floaty sliding feeling
        if (Mathf.Abs(moveInput) < 0.1f && isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;

        if (animator != null)
            animator.SetFloat("Run", Mathf.Abs(moveInput));
    }
    #endregion

    #region FOOTSTEPS
    public void PlayFootstep()
    {
        if (!isGrounded) return;
        if (footstepSounds.Length == 0) return;

        int index = Random.Range(0, footstepSounds.Length);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(footstepSounds[index], footstepVolume);
        else
            AudioSource.PlayClipAtPoint(footstepSounds[index], transform.position, footstepVolume);
    }
    #endregion

    #region JUMP
    private void HandleJump()
    {
        bool canJump = isGrounded || IsStandingOnEnemy();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                PerformJump();
            }
            else if (hasDoubleJump)
            {
                PerformJump();
                hasDoubleJump = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * jumpCutMultiplier
                );
            }
            isJumping = false;
        }
    }

    private bool IsStandingOnEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius + 0.1f,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            if (!hit.isTrigger)
                return true;
        }

        return false;
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayPlayerJump();
    }
    #endregion

    #region SLAM
    private void HandleSlam()
    {
        bool slamKeyPressed = Input.GetKeyDown(KeyCode.S) ||
                              Input.GetKeyDown(KeyCode.DownArrow);

        if (!slamKeyPressed) return;
        if (isSlamming) return;
        if (slamOnCooldown) return;

        // only block slam if clearly standing still on the ground
        // if the player has any upward or downward velocity they are in the air
        if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.1f) return;

        StartCoroutine(PerformSlam());
    }

    private bool IsStompInputHeld()
    {
        return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    }

    private IEnumerator PerformSlam()
    {
        isSlamming = true;
        slamOnCooldown = true;

        if (!isBuffActive)
            isInvincible = true;

        // force straight down
        rb.linearVelocity = new Vector2(0f, -slamForce);

        // wait for collision events to fire via OnCollisionEnter2D or OnTriggerEnter2D
        // slamLanded is set to true by those methods
        slamLanded = false;

        float timeout = 3f;
        float elapsed = 0f;

        while (!slamLanded && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isSlamming = false;

        yield return new WaitForSeconds(0.2f);

        if (!isBuffActive)
            isInvincible = false;

        yield return new WaitForSeconds(slamCooldownTime);
        slamOnCooldown = false;
    }
    #endregion

    #region BAGUETTE THROW
    private void HandleBaguetteThrow()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentBaguettes > 0 &&
                Time.time - lastThrowTime >= throwCooldown)
            {
                ThrowBaguette();
            }
        }
    }

    private void ThrowBaguette()
    {
        lastThrowTime = Time.time;
        currentBaguettes--;

        UpdateBaguetteUI();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBaguetteThrow();

        Vector2 throwDirection = spriteRenderer.flipX ?
            Vector2.left : Vector2.right;

        Vector3 spawnPosition = throwPoint.position;

        if (throwDirection == Vector2.left)
            spawnPosition.x = transform.position.x - Mathf.Abs(throwPoint.localPosition.x);
        else
            spawnPosition.x = transform.position.x + Mathf.Abs(throwPoint.localPosition.x);

        GameObject baguette = Instantiate(baguettePrefab, spawnPosition, Quaternion.identity);

        BaguetteProjectile projectile = baguette.GetComponent<BaguetteProjectile>();
        if (projectile != null)
            projectile.Launch(throwDirection, !isGrounded);
    }

    public bool AddBaguette()
    {
        RefreshBaguetteCapacity();

        if (currentBaguettes >= maxBaguettes) return false;

        currentBaguettes++;
        UpdateBaguetteUI();

        return true;
    }
    #endregion

    #region IDLE TIMER
    private void HandleIdleTimer()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isActive = isMoving || !isGrounded || isSlamming;

        if (isActive)
            idleTimer = 0f;
        else
            idleTimer += Time.deltaTime;

        if (animator != null)
            animator.SetFloat("IdleTime", idleTimer);
    }
    #endregion

    #region STOMP BUFF
    private void HandleBuff()
    {
        if (!isBuffActive) return;

        buffTimer -= Time.deltaTime;

        if (buffTimer <= 0)
            EndBuff();
    }

    public void RegisterStomp()
    {
        if (isBuffActive) return;

        currentStompChain++;
        Debug.Log("Stomp chain: " + currentStompChain + "/" + stompsRequired);

        if (currentStompChain >= stompsRequired)
            ActivateBuff();
    }

    public void ResetStompChain()
    {
        currentStompChain = 0;
        Debug.Log("Stomp chain reset!");
    }

    private void ActivateBuff()
    {
        isBuffActive = true;
        buffTimer = buffDuration;
        currentStompChain = 0;

        moveSpeed = originalMoveSpeed * buffSpeedMultiplier;
        sprintSpeed = originalSprintSpeed * buffSpeedMultiplier;
        jumpForce = originalJumpForce * buffJumpMultiplier;

        isInvincible = true;

        spriteRenderer.color = goldenColor;

        if (buffFlashCoroutine != null)
            StopCoroutine(buffFlashCoroutine);

        buffFlashCoroutine = StartCoroutine(BuffFlash());

        if (buffDisplay != null)
            buffDisplay.ShowBuff(buffDuration);

        Debug.Log("Buff activated!");
    }

    private void EndBuff()
    {
        isBuffActive = false;

        moveSpeed = originalMoveSpeed;
        sprintSpeed = originalSprintSpeed;
        jumpForce = originalJumpForce;

        isInvincible = false;

        spriteRenderer.color = originalColor;

        if (buffFlashCoroutine != null)
            StopCoroutine(buffFlashCoroutine);

        if (buffDisplay != null)
            buffDisplay.HideBuff();

        Debug.Log("Buff ended!");
    }

    private IEnumerator BuffFlash()
    {
        yield return new WaitForSeconds(buffDuration - 5f);

        while (isBuffActive)
        {
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = goldenColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    #region UI UPDATES
    private void UpdateHealthUI()
    {
        if (heartHealthBar != null)
            heartHealthBar.UpdateHearts(health, maxHealth);
    }

    private void UpdateCroissantUI()
    {
        if (croissantMeter != null)
            croissantMeter.UpdateCroissants(currentCroissantEnergy, maxCroissantEnergy);
    }
    #endregion

    #region DAMAGE AND DEATH
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Enemy2D uses a solid collider, so stomps can arrive as normal collisions.
        // A stomp only counts while S or Down Arrow is being held.
        if (isSlamming && IsStompInputHeld() && TryStompEnemy(collision.collider))
            return;

        // If the player hits or lands on a solid enemy without holding stomp input,
        // end the slam as a normal collision instead of damaging/stomping the enemy.
        if (isSlamming && IsEnemyCollider(collision.collider))
        {
            slamLanded = true;
            fallTimer = 0f;
            isFalling = false;
            return;
        }

        // normal damage tag
        if (collision.gameObject.CompareTag(DamageTag))
            TakeDamage();

        // slam landed on ground
        if (isSlamming && ((groundLayer.value & (1 << collision.gameObject.layer)) != 0))
        {
            slamLanded = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayPlayerFallHit();

            fallTimer = 0f;
            isFalling = false;
            return;
        }

        // normal landing after falling for long enough
        if (((groundLayer.value & (1 << collision.gameObject.layer)) != 0))
        {
            if (isFalling && fallTimer >= 0.5f)
            {
                Debug.Log("Landing sound! fallTimer: " + fallTimer);
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayPlayerFallHit();
            }

            fallTimer = 0f;
            isFalling = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kept for older trigger-based enemies and for projects that add a trigger stomp zone.
        if (isSlamming && IsStompInputHeld())
            TryStompEnemy(other);
    }

    private bool IsEnemyCollider(Collider2D enemyCollider)
    {
        if (enemyCollider == null) return false;
        if (enemyCollider.GetComponentInParent<Enemy2D>() != null) return true;

        return enemyCollider.CompareTag(EnemyTag) ||
               enemyCollider.transform.root.CompareTag(EnemyTag);
    }

    private bool TryStompEnemy(Collider2D enemyCollider)
    {
        if (enemyCollider == null) return false;
        if (!IsStompInputHeld()) return false;

        Enemy2D enemy2D = enemyCollider.GetComponentInParent<Enemy2D>();
        if (enemy2D != null)
        {
            bool stomped = enemy2D.TryStomp(this, rb, slamBounceForce);
            if (stomped)
            {
                slamLanded = true;
                fallTimer = 0f;
                isFalling = false;
            }
            return stomped;
        }

        // Fallback for your old trigger-based Enemy script without directly depending on its class name.
        bool isOldEnemy = enemyCollider.isTrigger &&
                          (enemyCollider.CompareTag(EnemyTag) || enemyCollider.transform.root.CompareTag(EnemyTag));

        if (!isOldEnemy) return false;

        GameObject receiver = enemyCollider.attachedRigidbody != null
            ? enemyCollider.attachedRigidbody.gameObject
            : enemyCollider.transform.root.gameObject;

        receiver.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
        receiver.SendMessage("Stun", SendMessageOptions.DontRequireReceiver);

        RegisterStomp();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, slamBounceForce);
        slamLanded = true;
        fallTimer = 0f;
        isFalling = false;

        return true;
    }

    public void TakeDamage(bool fromEnemy = false)
    {
        if (isDead || isInvincible) return;

        health -= 1;

        // Emit red damage particles
        if (damageParticles != null)
            damageParticles.EmitDamageParticles();

        ResetStompChain();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // only play hurt sound when hit by enemy
        if (fromEnemy && SoundManager.Instance != null)
            SoundManager.Instance.PlayPlayerHurt();

        StartCoroutine(BlinkRed());

        if (health <= 0)
            Die();
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        deathMenu.ToggleDeathScreen();
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, slamRadius);

        // draw the fall detection ray
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);
    }
    #endregion
}
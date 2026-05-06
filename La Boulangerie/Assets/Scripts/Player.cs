using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    #region STATS
    [Header("Stats")]
    public int health = 3;
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
    public float slamForce = 20f;
    public float slamBounceForce = 8f;
    public float slamRadius = 0.5f;
    public LayerMask enemyLayer;
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
    private bool isSlamming;
    private bool hasDoubleJump;
    private bool canDoubleJump;
    private float currentCroissantEnergy;

    private float originalMoveSpeed;
    private float originalSprintSpeed;
    private float originalJumpForce;

    private float idleTimer = 0f;

    private const int MaxHealth = 3;
    private const string DamageTag = "Damage";
    private const string EnemyTag = "Enemy";
    private const string StartScene = "Level1";
    private const string DoubleJumpScene = "Level2";
    #endregion

    #region START
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        originalColor = spriteRenderer.color;

        currentCroissantEnergy = maxCroissantEnergy;

        originalMoveSpeed = moveSpeed;
        originalSprintSpeed = sprintSpeed;
        originalJumpForce = jumpForce;

        string currentScene = SceneManager.GetActiveScene().name;
        canDoubleJump = (currentScene == DoubleJumpScene);

        if (currentScene == "Level1") killHeight = -20f;
        else if (currentScene == "Level2") killHeight = -20f;
        else if (currentScene == "Level3") killHeight = -30f;

        if (heartHealthBar != null)
            heartHealthBar.UpdateHearts(health);

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

            if (isSlamming)
            {
                isSlamming = false;
            }
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

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        if (animator != null)
        {
            animator.SetFloat("Run", Mathf.Abs(moveInput));
        }
    }
    #endregion

    public void PlayFootstep()
    {
        if (!isGrounded) return;

        if (footstepSounds.Length == 0) return;

        int index = Random.Range(0, footstepSounds.Length);

        AudioSource.PlayClipAtPoint(
            footstepSounds[index],
            transform.position,
            footstepVolume
        );
    }

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
    }
    #endregion

    #region SLAM
    private void HandleSlam()
    {
        bool slamKeyPressed = Input.GetKeyDown(KeyCode.S) ||
                              Input.GetKeyDown(KeyCode.DownArrow);

        bool isFallingOrJumping = Mathf.Abs(rb.linearVelocity.y) > 0.5f;

        if (slamKeyPressed && !isSlamming && !isGrounded && isFallingOrJumping)
        {
            StartCoroutine(PerformSlam());
            StartCoroutine(SlamInvincibility());
        }
    }

    private IEnumerator PerformSlam()
    {
        isSlamming = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slamForce);

        yield return new WaitUntil(() =>
            isGrounded ||
            Physics2D.OverlapCircle(groundCheck.position, slamRadius, enemyLayer)
        );

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            groundCheck.position,
            slamRadius,
            enemyLayer
        );

        bool hitEnemy = false;

        foreach (Collider2D col in hitColliders)
        {

            if (!col.isTrigger) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
                enemy.Stun();
                hitEnemy = true;
            }
        }

        if (hitEnemy)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, slamBounceForce);

            isSlamming = false;

            RegisterStomp();
        }
        else
        {

            isSlamming = false;
        }
    }

    private IEnumerator SlamInvincibility()
    {

        if (!isBuffActive)
        {
            isInvincible = true;
            yield return new WaitForSeconds(0.2f);

            if (!isBuffActive)
            {
                isInvincible = false;
            }
        }
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

        if (baguetteUI != null)
            baguetteUI.UpdateSlots(currentBaguettes);

        GameObject baguette = Instantiate(
            baguettePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        Vector2 throwDirection = spriteRenderer.flipX ?
            Vector2.left : Vector2.right;

        BaguetteProjectile projectile = baguette.GetComponent<BaguetteProjectile>();
        if (projectile != null)
        {
            projectile.Launch(throwDirection, !isGrounded);
        }
    }

    public bool AddBaguette()
    {

        if (currentBaguettes >= maxBaguettes) return false;

        currentBaguettes++;

        if (baguetteUI != null)
            baguetteUI.UpdateSlots(currentBaguettes);

        return true;
    }
    #endregion

    #region IDLE TIMER
    private void HandleIdleTimer()
    {

        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isActive = isMoving || !isGrounded || isSlamming;

        if (isActive)
        {

            idleTimer = 0f;
        }
        else
        {

            idleTimer += Time.deltaTime;
        }

        if (animator != null)
        {
            animator.SetFloat("IdleTime", idleTimer);
        }
    }
    #endregion

    #region STOMP BUFF
    private void HandleBuff()
    {

        if (!isBuffActive) return;

        buffTimer -= Time.deltaTime;

        if (buffTimer <= 0)
        {
            EndBuff();
        }
    }

    public void RegisterStomp()
    {

        if (isBuffActive) return;

        currentStompChain++;
        Debug.Log("Stomp chain: " + currentStompChain + "/" + stompsRequired);

        if (currentStompChain >= stompsRequired)
        {
            ActivateBuff();
        }
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
        {
            heartHealthBar.UpdateHearts(health);
        }
    }

    private void UpdateCroissantUI()
    {

        if (croissantMeter != null)
        {
            croissantMeter.UpdateCroissants(currentCroissantEnergy, maxCroissantEnergy);
        }
    }
    #endregion

    #region DAMAGE AND DEATH
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag(DamageTag))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {

        if (isDead || isInvincible) return;

        health -= 1;

        ResetStompChain();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        StartCoroutine(BlinkRed());

        if (health <= 0)
        {
            Die();
        }
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
    }
    #endregion
}

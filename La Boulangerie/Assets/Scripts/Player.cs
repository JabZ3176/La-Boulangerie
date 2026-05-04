using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    #region STATS
    [Header("Stats")]
    public int health = 3;              // how many hits the player can take
    private bool isDead = false;        // stops death from firing more than once
    private bool isInvincible = false;  // true during a slam so player cant take damage
    #endregion

    #region MOVEMENT
    [Header("Movement")]
    public float moveSpeed = 5f;            // normal walking speed
    public float sprintSpeed = 9f;          // speed when holding shift
    public float jumpForce = 10f;           // how high the player jumps
    public float jumpCutMultiplier = 0.5f;  // reduces jump height if space released early
    #endregion

    #region SLAM
    [Header("Slam")]
    public float slamForce = 20f;       // how fast the player slams downward
    public float slamBounceForce = 8f;  // how high the player bounces after a slam
    public float slamRadius = 0.5f;     // how wide the hit detection is below the player
    public LayerMask enemyLayer;        // set this to your Enemy layer in the Inspector
    #endregion

    #region BAGUETTE ENERGY
    [Header("Baguette Energy Meter")]
    public float maxBaguetteEnergy = 100f;      // maximum sprint energy
    public float baguetteDrainRate = 20f;       // how fast energy drains while sprinting
    public float baguetteRechargeRate = 10f;    // how fast energy recharges when not sprinting
    public float baguetteMinToSprint = 10f;     // minimum energy needed to start sprinting
    #endregion

    #region BAGUETTE THROWING
    [Header("Baguette Throwing")]
    public GameObject baguettePrefab;   // drag your baguette prefab here
    public Transform throwPoint;        // empty object at the player's hand position
    public float throwCooldown = 0.5f;  // seconds between throws
    public int maxBaguettes = 3;        // maximum baguettes the player can carry
    public BaguetteUI baguetteUI;       // drag your BaguetteUI object here

    private int currentBaguettes = 0;   // how many baguettes the player currently has
    private float lastThrowTime = -1f;  // tracks when the player last threw
    #endregion

    #region STOMP BUFF
    [Header("Stomp Buff")]
    public int stompsRequired = 5;           // how many stomps needed to trigger buff
    public float buffDuration = 15f;         // how long the buff lasts in seconds
    public float buffSpeedMultiplier = 1.2f; // speed multiplier during buff
    public float buffJumpMultiplier = 1.2f;  // jump multiplier during buff
    public BuffDisplay buffDisplay;          // drag your BuffDisplay object here

    private int currentStompChain = 0;      // tracks consecutive stomps
    private bool isBuffActive = false;      // is the buff currently active
    private float buffTimer = 0f;           // counts down the buff duration
    private Color goldenColor = new Color(1f, 0.84f, 0f, 1f); // golden colour
    private Coroutine buffFlashCoroutine;   // reference to the flash coroutine
    #endregion

    #region GROUND DETECTION
    [Header("Ground Detection")]
    public Transform groundCheck;           // empty object placed at the player's feet
    public float groundCheckRadius = 0.2f;  // how wide the ground detection circle is
    public LayerMask groundLayer;           // set this to your Ground layer in the Inspector
    #endregion

    #region UI
    [Header("UI")]
    public HeartHealthBar heartHealthBar;   // drag HealthBar object here
    public Image baguetteEnergyImage;       // drag energy bar image here
    #endregion

    #region REFERENCES
    [Header("References")]
    public DeathMenu deathMenu;     // reference to the death menu script
    #endregion

    #region PRIVATE VARIABLES
    private Rigidbody2D rb;                 // the player's rigidbody
    private SpriteRenderer spriteRenderer;  // the player's sprite renderer
    private Animator animator;              // reference to the player's animator
    private Color originalColor;            // stores the original sprite color
    private float killHeight = -10f;        // y position that kills the player

    private bool isGrounded;        // is the player touching the ground
    private bool isJumping;         // is the player currently jumping
    private bool isSprinting;       // is the player currently sprinting
    private bool isSlamming;        // is the player currently performing a slam
    private bool hasDoubleJump;     // does the player currently have a double jump available
    private bool canDoubleJump;     // is double jump unlocked in this scene
    private float currentBaguetteEnergy; // current sprint energy amount

    // stores the original movement values so we can always restore them correctly
    private float originalMoveSpeed;
    private float originalSprintSpeed;
    private float originalJumpForce;

    // idle animation timer
    private float idleTimer = 0f;   // tracks how long the player has been still

    // constants — these never change at runtime
    private const int MaxHealth = 3;
    private const string DamageTag = "Damage";
    private const string EnemyTag = "Enemy";
    private const string StartScene = "Level1";
    private const string DoubleJumpScene = "Level2";
    #endregion

    #region START
    void Start()
    {
        // get components attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // save the original sprite color so we can return to it after flashing
        originalColor = spriteRenderer.color;

        // fill baguette energy to max at the start
        currentBaguetteEnergy = maxBaguetteEnergy;

        // save original movement values before anything modifies them
        originalMoveSpeed = moveSpeed;
        originalSprintSpeed = sprintSpeed;
        originalJumpForce = jumpForce;

        // check which scene we are in and set up accordingly
        string currentScene = SceneManager.GetActiveScene().name;
        canDoubleJump = (currentScene == DoubleJumpScene);

        // set the kill height per scene so falling off works in every level
        if (currentScene == "Level1") killHeight = -10f;
        else if (currentScene == "Level2") killHeight = -20f;
        else if (currentScene == "Level3") killHeight = -30f;

        // show full hearts at the start
        if (heartHealthBar != null)
            heartHealthBar.UpdateHearts(health);
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
        UpdateBaguetteUI();

        // kill the player if they fall below the kill height for this scene
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

        // draw a small circle at the player's feet to check if touching the ground
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // when the player lands restore the double jump if unlocked
        if (!wasGrounded && isGrounded)
        {
            hasDoubleJump = canDoubleJump;

            // if we were slamming and just landed stop the slam state
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

        if (shiftHeld && currentBaguetteEnergy >= baguetteMinToSprint)
        {
            // drain energy while sprinting
            isSprinting = true;
            currentBaguetteEnergy -= baguetteDrainRate * Time.deltaTime;
            currentBaguetteEnergy = Mathf.Max(currentBaguetteEnergy, 0f);
        }
        else
        {
            // recharge energy when not sprinting
            isSprinting = false;
            currentBaguetteEnergy += baguetteRechargeRate * Time.deltaTime;
            currentBaguetteEnergy = Mathf.Min(currentBaguetteEnergy, maxBaguetteEnergy);
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

        // tell the animator how fast the player is moving
        // Mathf.Abs gives us a positive number regardless of direction
        if (animator != null)
        {
            animator.SetFloat("Run", Mathf.Abs(moveInput));
        }
    }
    #endregion

    #region JUMP
    private void HandleJump()
    {
        // allow jumping even when on top of an enemy
        bool canJump = isGrounded || IsStandingOnEnemy();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                PerformJump(); // normal jump from the ground or enemy top
            }
            else if (hasDoubleJump)
            {
                PerformJump();          // double jump in the air
                hasDoubleJump = false;  // use up the double jump
            }
        }

        // if space is released early cut the jump short for better game feel
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
        // check if there is an enemy directly below the player
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius + 0.1f,
            enemyLayer
        );

        // return true if any physical enemy collider is found below
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

        // check the player has meaningful vertical velocity
        // this stops the slam triggering when standing still on top of an enemy
        bool isFallingOrJumping = Mathf.Abs(rb.linearVelocity.y) > 0.5f;

        if (slamKeyPressed && !isSlamming && !isGrounded && isFallingOrJumping)
        {
            StartCoroutine(PerformSlam());
        }
    }

    private IEnumerator PerformSlam()
    {
        isSlamming = true;
        isInvincible = true; // player cant take damage during a slam

        // force the player straight down at slam speed
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slamForce);

        // wait until the player hits the ground or lands on an enemy
        yield return new WaitUntil(() =>
            isGrounded ||
            Physics2D.OverlapCircle(groundCheck.position, slamRadius, enemyLayer)
        );

        // check for any enemy trigger colliders within the slam radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            groundCheck.position,
            slamRadius,
            enemyLayer
        );

        bool hitEnemy = false;

        foreach (Collider2D col in hitColliders)
        {
            // skip the physical blocker — only process trigger colliders
            if (!col.isTrigger) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // deal 1 damage to the enemy
                enemy.Stun();        // stun the enemy temporarily
                hitEnemy = true;
            }
        }

        if (hitEnemy)
        {
            // bounce upward immediately after hitting an enemy
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, slamBounceForce);

            // mark slam as done immediately so player can chain slam again
            isSlamming = false;

            // register the stomp for the buff chain
            RegisterStomp();

            // brief grace period so OnTriggerStay doesnt immediately deal damage
            yield return new WaitForSeconds(0.2f);
            isInvincible = false;
        }
        else
        {
            // hit the ground not an enemy
            isSlamming = false;
            yield return new WaitForSeconds(0.2f);
            isInvincible = false;
        }
    }
    #endregion

    #region BAGUETTE THROW
    private void HandleBaguetteThrow()
    {
        // check for Z key press
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // must have baguettes and cooldown must have passed
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

        // update the UI to reflect the new ammo count
        if (baguetteUI != null)
            baguetteUI.UpdateSlots(currentBaguettes);

        // spawn the baguette prefab at the throw point
        GameObject baguette = Instantiate(
            baguettePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        // get the direction the player is facing
        Vector2 throwDirection = spriteRenderer.flipX ?
            Vector2.left : Vector2.right;

        // launch the baguette — pass isJumping so it knows to add upward velocity
        BaguetteProjectile projectile = baguette.GetComponent<BaguetteProjectile>();
        if (projectile != null)
        {
            projectile.Launch(throwDirection, !isGrounded);
        }
    }

    public bool AddBaguette()
    {
        // return false if already at max so pickup stays in the world
        if (currentBaguettes >= maxBaguettes) return false;

        currentBaguettes++;

        // update the UI
        if (baguetteUI != null)
            baguetteUI.UpdateSlots(currentBaguettes);

        return true;
    }
    #endregion

    #region IDLE TIMER
    private void HandleIdleTimer()
    {
        // check if the player is moving or doing anything
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isActive = isMoving || !isGrounded || isSlamming;

        if (isActive)
        {
            // reset the timer whenever the player moves or jumps
            idleTimer = 0f;
        }
        else
        {
            // count up while the player is completely still
            idleTimer += Time.deltaTime;
        }

        // pass the idle time to the animator so it knows when to switch
        if (animator != null)
        {
            animator.SetFloat("IdleTime", idleTimer);
        }
    }
    #endregion

    #region STOMP BUFF
    private void HandleBuff()
    {
        // only run if buff is active
        if (!isBuffActive) return;

        buffTimer -= Time.deltaTime;

        if (buffTimer <= 0)
        {
            EndBuff();
        }
    }

    public void RegisterStomp()
    {
        // dont stack stomps if buff is already active
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
        // reset the chain back to zero
        currentStompChain = 0;
        Debug.Log("Stomp chain reset!");
    }

    private void ActivateBuff()
    {
        isBuffActive = true;
        buffTimer = buffDuration;
        currentStompChain = 0;

        // apply buff using original values so we never compound the multiplier
        moveSpeed = originalMoveSpeed * buffSpeedMultiplier;
        sprintSpeed = originalSprintSpeed * buffSpeedMultiplier;
        jumpForce = originalJumpForce * buffJumpMultiplier;

        // make player invincible during buff
        isInvincible = true;

        // turn the player golden
        spriteRenderer.color = goldenColor;

        // stop any existing flash coroutine before starting a new one
        if (buffFlashCoroutine != null)
            StopCoroutine(buffFlashCoroutine);

        buffFlashCoroutine = StartCoroutine(BuffFlash());

        // show the buff UI countdown
        if (buffDisplay != null)
            buffDisplay.ShowBuff(buffDuration);

        Debug.Log("Buff activated!");
    }

    private void EndBuff()
    {
        isBuffActive = false;

        // restore directly from saved originals to avoid floating point errors
        moveSpeed = originalMoveSpeed;
        sprintSpeed = originalSprintSpeed;
        jumpForce = originalJumpForce;

        // remove invincibility
        isInvincible = false;

        // return to original colour
        spriteRenderer.color = originalColor;

        // stop the flash coroutine
        if (buffFlashCoroutine != null)
            StopCoroutine(buffFlashCoroutine);

        // hide the buff UI
        if (buffDisplay != null)
            buffDisplay.HideBuff();

        Debug.Log("Buff ended!");
    }

    private IEnumerator BuffFlash()
    {
        // wait until the last 5 seconds of the buff before flashing
        yield return new WaitForSeconds(buffDuration - 5f);

        // flash between golden and original colour every 0.2 seconds
        while (isBuffActive)
        {
            spriteRenderer.color = originalColor;   // flash to normal
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = goldenColor;     // flash back to gold
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    #region UI UPDATES
    private void UpdateHealthUI()
    {
        // tell the heart bar to update based on current health
        if (heartHealthBar != null)
        {
            heartHealthBar.UpdateHearts(health);
        }
    }

    private void UpdateBaguetteUI()
    {
        // update the energy bar fill amount
        if (baguetteEnergyImage != null)
        {
            baguetteEnergyImage.fillAmount = currentBaguetteEnergy / maxBaguetteEnergy;
        }
    }
    #endregion

    #region DAMAGE AND DEATH
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if the player touches anything tagged Damage they take a hit
        if (collision.gameObject.CompareTag(DamageTag))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        // dont take damage if already dead or invincible
        if (isDead || isInvincible) return;

        health -= 1;

        // reset the stomp chain when player takes damage
        ResetStompChain();

        // knock the player upward slightly when hit
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // flash red briefly to show the player was hit
        StartCoroutine(BlinkRed());

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;       // flash red
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;   // return to original color
    }

    private void Die()
    {
        // guard so die can only ever run once even if called multiple times
        if (isDead) return;
        isDead = true;
        deathMenu.ToggleDeathScreen();
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        // draw the ground check circle in green
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // draw the slam radius circle in red so you can see exactly what it detects
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, slamRadius);
    }
    #endregion
}
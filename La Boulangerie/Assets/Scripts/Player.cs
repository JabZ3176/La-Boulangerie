using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // STATS
    // ─────────────────────────────────────────────
    [Header("Stats")]
    public int health = 3;              // how many hits the player can take
    private bool isDead = false;        // stops death from firing more than once
    private bool isInvincible = false;  // true during a slam so player cant take damage

    // ─────────────────────────────────────────────
    // MOVEMENT
    // ─────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 5f;            // normal walking speed
    public float sprintSpeed = 9f;          // speed when holding shift
    public float jumpForce = 10f;           // how high the player jumps
    public float jumpCutMultiplier = 0.5f;  // reduces jump height if space released early

    // ─────────────────────────────────────────────
    // SLAM
    // ─────────────────────────────────────────────
    [Header("Slam")]
    public float slamForce = 20f;       // how fast the player slams downward
    public float slamBounceForce = 8f;  // how high the player bounces after a slam
    public float slamRadius = 0.5f;     // how wide the hit detection is below the player
    public LayerMask enemyLayer;        // set this to your Enemy layer in the Inspector

    // ─────────────────────────────────────────────
    // BAGUETTE ENERGY
    // ─────────────────────────────────────────────
    [Header("Baguette Energy Meter")]
    public float maxBaguetteEnergy = 100f;      // maximum sprint energy
    public float baguetteDrainRate = 20f;       // how fast energy drains while sprinting
    public float baguetteRechargeRate = 10f;    // how fast energy recharges when not sprinting
    public float baguetteMinToSprint = 10f;     // minimum energy needed to start sprinting

    // ─────────────────────────────────────────────
    // BAGUETTE THROWING
    // ─────────────────────────────────────────────
    [Header("Baguette Throwing")]
    public GameObject baguettePrefab;   // drag your baguette prefab here
    public Transform throwPoint;        // empty object at the player's hand position
    public float throwCooldown = 0.5f;  // seconds between throws
    public int maxBaguettes = 3;        // maximum baguettes the player can carry
    public BaguetteUI baguetteUI;       // drag your BaguetteUI object here

    private int currentBaguettes = 0;   // how many baguettes the player currently has
    private float lastThrowTime = -1f;  // tracks when the player last threw

    // ─────────────────────────────────────────────
    // GROUND DETECTION
    // ─────────────────────────────────────────────
    [Header("Ground Detection")]
    public Transform groundCheck;           // empty object placed at the player's feet
    public float groundCheckRadius = 0.2f;  // how wide the ground detection circle is
    public LayerMask groundLayer;           // set this to your Ground layer in the Inspector

    // ─────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────
    [Header("UI")]
    public HeartHealthBar heartHealthBar;   // drag HealthBar object here
    public Image baguetteEnergyImage;

    // ─────────────────────────────────────────────
    // IDLE ANIMATION
    // ─────────────────────────────────────────────
    private Animator animator;          // reference to the player's animator
    private float idleTimer = 0f;       // tracks how long the player has been still
    private float idleThreshold = 10f;  // seconds before idle bob starts

    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("References")]
    public DeathMenu deathMenu;     // reference to the death menu script

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private Rigidbody2D rb;                 // the player's rigidbody
    private SpriteRenderer spriteRenderer;  // the player's sprite renderer
    private Color originalColor;            // stores the original sprite color
    private float killHeight = -10f;        // y position that kills the player

    private bool isGrounded;        // is the player touching the ground
    private bool isJumping;         // is the player currently jumping
    private bool isSprinting;       // is the player currently sprinting
    private bool isSlamming;        // is the player currently performing a slam
    private bool hasDoubleJump;     // does the player currently have a double jump available
    private bool canDoubleJump;     // is double jump unlocked in this scene
    private float currentBaguetteEnergy; // current sprint energy amount

    // constants — these never change at runtime
    private const int MaxHealth = 3;
    private const string DamageTag = "Damage";
    private const string EnemyTag = "Enemy";
    private const string StartScene = "Level1";
    private const string DoubleJumpScene = "Level2";

    // ─────────────────────────────────────────────
    // START — runs once when the scene loads
    // ─────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentBaguetteEnergy = maxBaguetteEnergy;
        // get the animator component on the player
        animator = GetComponent<Animator>();

        string currentScene = SceneManager.GetActiveScene().name;
        canDoubleJump = (currentScene == DoubleJumpScene);

        if (currentScene == "Level1") killHeight = -10f;
        else if (currentScene == "Level2") killHeight = -20f;
        else if (currentScene == "Level3") killHeight = -30f;

        // show full hearts at the start
        if (heartHealthBar != null)
        {
            heartHealthBar.UpdateHearts(health);
        }
    }

    // ─────────────────────────────────────────────
    // UPDATE — runs every frame
    // ─────────────────────────────────────────────
    void Update()
    {
        HandleSprint();
        HandleMovement();
        HandleJump();
        HandleSlam();
        HandleBaguetteThrow();
        HandleIdleTimer();      // add this line
        UpdateHealthUI();
        UpdateBaguetteUI();

        if (transform.position.y < killHeight)
        {
            Die();
        }
    }

    // ─────────────────────────────────────────────
    // FIXED UPDATE — runs on physics ticks
    // ─────────────────────────────────────────────
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

    // ─────────────────────────────────────────────
    // SPRINT
    // ─────────────────────────────────────────────
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

    // ─────────────────────────────────────────────
    // MOVEMENT
    // ─────────────────────────────────────────────
    private void HandleMovement()
    {
        // dont allow movement control while slamming
        if (isSlamming) return;

        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // apply horizontal movement while keeping vertical velocity unchanged
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // flip the sprite to face the direction the player is moving
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false; // facing right
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;  // facing left
        }
    }

    // ─────────────────────────────────────────────
    // JUMP
    // ─────────────────────────────────────────────
    private void HandleJump()
    {
        // dont allow jumping while slamming
        if (isSlamming) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                PerformJump(); // normal jump from the ground
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

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
    }

    // ─────────────────────────────────────────────
    // SLAM
    // ─────────────────────────────────────────────
    private void HandleSlam()
    {
        // slam can only be performed when:
        // the player is in the air, not already slamming, and presses S or down arrow
        bool slamKeyPressed = Input.GetKeyDown(KeyCode.S) ||
                              Input.GetKeyDown(KeyCode.DownArrow);

        if (slamKeyPressed && !isGrounded && !isSlamming)
        {
            StartCoroutine(PerformSlam());
        }
    }


    private IEnumerator PerformSlam()
    {
        isSlamming = true;
        isInvincible = true;

        // force the player straight down at slam speed
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slamForce);

        // wait until the player hits the ground OR lands on an enemy
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
            // skip the physical blocker — only process the trigger collider
            if (!col.isTrigger) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
                enemy.Stun();
                hitEnemy = true;
            }
        }

        // only bounce if we actually hit an enemy
        if (hitEnemy)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, slamBounceForce);
        }

        isSlamming = false;

        // brief grace period so OnTriggerStay doesnt immediately deal damage
        yield return new WaitForSeconds(0.2f);
        isInvincible = false;
    }

    // ─────────────────────────────────────────────
    // BAGUETTE THROW
    // ─────────────────────────────────────────────
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

    // ─────────────────────────────────────────────
    // IDLE TIMER — tracks how long player has been still
    // ─────────────────────────────────────────────
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

    // ─────────────────────────────────────────────
    // ADD BAGUETTE — called from BaguettePickup.cs
    // ─────────────────────────────────────────────
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

    // ─────────────────────────────────────────────
    // UI UPDATES
    // ─────────────────────────────────────────────
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
        baguetteEnergyImage.fillAmount = currentBaguetteEnergy / maxBaguetteEnergy;
    }

    // ─────────────────────────────────────────────
    // DAMAGE AND DEATH
    // ─────────────────────────────────────────────
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
        // dont take damage if already dead or currently invincible during a slam
        if (isDead || isInvincible) return;

        health -= 1;

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
}
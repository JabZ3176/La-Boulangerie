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

    // ─────────────────────────────────────────────
    // MOVEMENT
    // ─────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 5f;        // normal walking speed
    public float sprintSpeed = 9f;      // speed when holding shift
    public float jumpForce = 10f;       // how high the player jumps
    public float jumpCutMultiplier = 0.5f; // reduces jump height if space released early

    // ─────────────────────────────────────────────
    // SLAM
    // ─────────────────────────────────────────────
    [Header("Slam")]
    public float slamForce = 20f;           // how fast the player slams downward
    public float slamBounceForce = 8f;      // how high the player bounces after a slam
    public float slamRadius = 0.5f;         // how wide the hit detection is below the player
    public LayerMask enemyLayer;            // set this to your Enemy layer in the Inspector

    // ─────────────────────────────────────────────
    // BAGUETTE ENERGY
    // ─────────────────────────────────────────────
    [Header("Baguette Energy Meter")]
    public float maxBaguetteEnergy = 100f;      // maximum sprint energy
    public float baguetteDrainRate = 20f;       // how fast energy drains while sprinting
    public float baguetteRechargeRate = 10f;    // how fast energy recharges when not sprinting
    public float baguetteMinToSprint = 10f;     // minimum energy needed to start sprinting

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
    public Image healthImage;           // health bar image (fill type)
    public Image baguetteEnergyImage;   // baguette energy bar image (fill type)

    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("References")]
    public GameObject container;        // any UI container if needed
    public DeathMenu deathMenu;         // reference to the death menu script

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
        // get components attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // save the original sprite color so we can return to it after flashing
        originalColor = spriteRenderer.color;

        // fill the baguette energy to max at the start
        currentBaguetteEnergy = maxBaguetteEnergy;

        // check which scene we are in and set up accordingly
        string currentScene = SceneManager.GetActiveScene().name;
        canDoubleJump = (currentScene == DoubleJumpScene);

        // set the kill height based on the current scene
        if (currentScene == "Level1") killHeight = -10f;
        else if (currentScene == "Level2") killHeight = -20f;
        else if (currentScene == "Level3") killHeight = -30f;
    }

    // ─────────────────────────────────────────────
    // UPDATE — runs every frame
    // ─────────────────────────────────────────────
    void Update()
    {
        HandleSprint();
        HandleMovement();
        HandleJump();
        HandleSlam();      // check for slam input every frame
        UpdateHealthUI();
        UpdateBaguetteUI();

        // kill the player if they fall below the kill height
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

        // draw a small circle at the player's feet to check if they are on the ground
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // when the player lands restore double jump if unlocked
        if (!wasGrounded && isGrounded)
        {
            hasDoubleJump = canDoubleJump;

            // if we were slamming and just landed, stop the slam
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
        // dont allow movement control during a slam
        if (isSlamming) return;

        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // apply horizontal movement while keeping vertical velocity unchanged
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    // ─────────────────────────────────────────────
    // JUMP
    // ─────────────────────────────────────────────
    private void HandleJump()
    {
        // dont allow jumping during a slam
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

        // if space is released early, cut the jump short
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
        // - the player is in the air (not grounded)
        // - not already slamming
        // - pressing S or down arrow
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

        // slam straight down by overriding vertical velocity
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slamForce);

        // wait until the player hits the ground before checking for enemies
        yield return new WaitUntil(() => isGrounded);

        // once landed check for enemies directly below using a circle overlap
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            groundCheck.position, // check from the player's feet
            slamRadius,
            enemyLayer            // only detect objects on the enemy layer
        );

        // if any enemies were found underneath, hit them all
        if (hitEnemies.Length > 0)
        {
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1); // deal 1 damage to the enemy
                    enemy.Stun();        // stun the enemy temporarily
                }
            }

            // bounce the player upward after a successful slam on an enemy
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, slamBounceForce);
        }

        isSlamming = false;
    }

    // ─────────────────────────────────────────────
    // UI UPDATES
    // ─────────────────────────────────────────────
    private void UpdateHealthUI()
    {
        // fill amount goes from 0 to 1 based on current health vs max health
        healthImage.fillAmount = (float)health / MaxHealth;
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
        // dont take damage if already dead
        if (isDead) return;

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
        spriteRenderer.color = Color.red;           // flash red
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;       // return to original color
    }

    private void Die()
    {
        // guard so die can only run once even if called multiple times
        if (isDead) return;
        isDead = true;
        deathMenu.ToggleDeathScreen();
    }
}
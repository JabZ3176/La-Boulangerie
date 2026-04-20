using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 9f;              // Speed while sprinting
    public float jumpForce = 10f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Baguette Energy Meter")]
    public float maxBaguetteEnergy = 100f;
    public float baguetteDrainRate = 20f;       // Energy lost per second while sprinting
    public float baguetteRechargeRate = 10f;    // Energy gained per second while not sprinting
    public float baguetteMinToSprint = 10f;     // Minimum energy required to start a sprint

    [Header("Power Slam")]
    public float slamForce = 20f;               // Downward force applied during the slam
    public float slamStunDuration = 2f;         // How long enemies stay stunned after being hit
    public float slamHitRadius = 1.5f;          // Radius of the slam impact zone below the player
    public LayerMask enemyLayer;                // Set this to your enemy layer in the Inspector

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("UI")]
    public Image healthImage;
    public Image baguetteEnergyImage;           // Assign your energy bar Image here in the Inspector

    [Header("Combat")]
    public float stunDuration = 1.5f;       // How long the enemy is stunned when the player touches it

    [Header("References")]
    public GameObject container;
    public DeathMenu deathMenu;

    // Private references
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // State tracking
    private bool isGrounded;
    private bool isJumping;
    private bool isSprinting;
    private bool isSlamming;                    // True while the slam is in progress
    private bool hasDoubleJump;                 // Whether the player currently has a double jump available
    private bool canDoubleJump;                 // Whether double jump is unlocked (true from Level2 onwards)
    private float currentBaguetteEnergy;        // Current energy value

    private const int MaxHealth = 3;
    private const string DamageTag = "Damage";
    private const string EnemyTag = "Enemy";
    private const string StartScene = "Level1";
    private const string DoubleJumpScene = "Level2";

    // -------------------------------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start with a full baguette
        currentBaguetteEnergy = maxBaguetteEnergy;

        // Unlock double jump if we're in Level2 or beyond
        string currentScene = SceneManager.GetActiveScene().name;
        canDoubleJump = (currentScene == DoubleJumpScene);
    }

    void Update()
    {
        HandleSprint();
        HandleMovement();
        HandleJump();
        UpdateHealthUI();
        UpdateBaguetteUI();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        // Restore the double jump charge whenever the player lands normally
        if (!wasGrounded && isGrounded)
        {
            hasDoubleJump = canDoubleJump;
        }
    }

    // -------------------------------------------------------------------------

    private void HandleSprint()
    {
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftHeld && currentBaguetteEnergy >= baguetteMinToSprint)
        {
            // Drain the baguette while sprinting
            isSprinting = true;
            currentBaguetteEnergy -= baguetteDrainRate * Time.deltaTime;
            currentBaguetteEnergy = Mathf.Max(currentBaguetteEnergy, 0f);
        }
        else
        {
            // Recharge the baguette while not sprinting
            isSprinting = false;
            currentBaguetteEnergy += baguetteRechargeRate * Time.deltaTime;
            currentBaguetteEnergy = Mathf.Min(currentBaguetteEnergy, maxBaguetteEnergy);
        }
    }

    private void HandleMovement()
    {
        // Freeze horizontal movement during a slam so the player drops straight down
        if (isSlamming) return;

        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        // Prevent jumping or double jumping while a slam is active
        if (isSlamming) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                // Normal jump from the ground
                PerformJump();
            }
            else if (hasDoubleJump)
            {
                // Mid-air double jump — consume the charge
                PerformJump();
                hasDoubleJump = false;
            }
        }

        // Cut the jump short if Space is released early (variable jump height)
        if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }

            isJumping = false;
        }
    }


    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
    }

    private void UpdateHealthUI()
    {
        healthImage.fillAmount = (float)health / MaxHealth;
    }

    private void UpdateBaguetteUI()
    {
        baguetteEnergyImage.fillAmount = currentBaguetteEnergy / maxBaguetteEnergy;
    }

    // -------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(DamageTag))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health -= 1;

        // Knock the player upward on hit
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
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        deathMenu.ToggleDeathScreen();
    }
}
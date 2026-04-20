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
    public float sprintSpeed = 9f;              
    public float jumpForce = 10f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Baguette Energy Meter")]
    public float maxBaguetteEnergy = 100f;
    public float baguetteDrainRate = 20f;       
    public float baguetteRechargeRate = 10f;    
    public float baguetteMinToSprint = 10f;     

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("UI")]
    public Image healthImage;
    public Image baguetteEnergyImage;                

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
    private bool isSlamming;                    
    private bool hasDoubleJump;                 
    private bool canDoubleJump;                 
    private float currentBaguetteEnergy;        

    private const int MaxHealth = 3;
    private const string DamageTag = "Damage";
    private const string EnemyTag = "Enemy";
    private const string StartScene = "Level1";
    private const string DoubleJumpScene = "Level2";


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentBaguetteEnergy = maxBaguetteEnergy;

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


        if (!wasGrounded && isGrounded)
        {
            hasDoubleJump = canDoubleJump;
        }
    }


    private void HandleSprint()
    {
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftHeld && currentBaguetteEnergy >= baguetteMinToSprint)
        {
            isSprinting = true;
            currentBaguetteEnergy -= baguetteDrainRate * Time.deltaTime;
            currentBaguetteEnergy = Mathf.Max(currentBaguetteEnergy, 0f);
        }
        else
        {
            isSprinting = false;
            currentBaguetteEnergy += baguetteRechargeRate * Time.deltaTime;
            currentBaguetteEnergy = Mathf.Min(currentBaguetteEnergy, maxBaguetteEnergy);
        }
    }

    private void HandleMovement()
    {
        if (isSlamming) return;

        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (isSlamming) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
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
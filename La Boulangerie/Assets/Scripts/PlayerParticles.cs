using UnityEngine;

public class PlayerMoveParticles : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem moveDust;
    [SerializeField] private ParticleSystem runStreaks;
    [SerializeField] private ParticleSystem landBurst;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Detection")]
    [SerializeField] private float minMoveSpeed = 0.15f;
    [SerializeField] private float runSpeedThreshold = 3.5f;
    [SerializeField] private bool requireGrounded = true;

    [Header("Dust Emission")]
    [SerializeField] private int particlesPerStep = 2;
    [SerializeField] private float stepInterval = 0.12f;

    [Header("Landing")]
    [SerializeField] private int landBurstAmount = 8;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.08f;
    [SerializeField] private LayerMask groundLayer;

    private float stepTimer;
    private bool wasGrounded;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rb == null) return;

        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > minMoveSpeed;
        bool grounded = IsGrounded();

        if (moveDust != null && isMoving && (!requireGrounded || grounded))
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                EmitMoveDust();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > runSpeedThreshold;

        if (runStreaks != null && isRunning && grounded)
        {
            EmitRunStreak();
        }

        if (!wasGrounded && grounded)
        {
            if (landBurst != null)
            {
                landBurst.Emit(landBurstAmount);
            }
        }

        wasGrounded = grounded;
    }

    private void EmitMoveDust()
    {
        var emitParams = new ParticleSystem.EmitParams();

        float direction = Mathf.Sign(rb.linearVelocity.x);

        emitParams.velocity = new Vector3(
            -direction * Random.Range(0.25f, 0.6f),
            Random.Range(0.05f, 0.18f),
            0f
        );

        emitParams.startSize = Random.Range(0.4f, 0.7f);
        emitParams.startLifetime = Random.Range(0.25f, 0.55f);
        emitParams.rotation = Random.Range(0f, 360f);

        moveDust.Emit(emitParams, particlesPerStep);
    }

    private void EmitRunStreak()
    {
        var emitParams = new ParticleSystem.EmitParams();

        float direction = Mathf.Sign(rb.linearVelocity.x);

        emitParams.velocity = new Vector3(
            -direction * Random.Range(0.4f, 0.9f),
            Random.Range(-0.03f, 0.06f),
            0f
        );

        emitParams.startSize = Random.Range(0.4f, 0.7f);
        emitParams.startLifetime = Random.Range(0.12f, 0.28f);
        emitParams.rotation = Random.Range(0f, 360f);

        runStreaks.Emit(emitParams, 1);
    }

    private bool IsGrounded()
    {
        if (!requireGrounded) return true;
        if (groundCheck == null) return true;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}
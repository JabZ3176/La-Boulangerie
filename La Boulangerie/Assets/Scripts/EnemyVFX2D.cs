using UnityEngine;

public class EnemyVFX2D : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem moveDust;
    [SerializeField] private ParticleSystem bloodParticles;

    [Header("Move Dust")]
    [SerializeField] private int moveDustCount = 2;
    [SerializeField] private Vector2 moveDustSpeedRange = new Vector2(0.25f, 0.7f);
    [SerializeField] private Vector2 moveDustSizeRange = new Vector2(0.08f, 0.18f);
    [SerializeField] private Vector2 moveDustLifetimeRange = new Vector2(0.18f, 0.38f);
    [SerializeField] private Vector2 moveDustUpRange = new Vector2(0.03f, 0.18f);

    [Header("Blood Particles")]
    [SerializeField] private int bloodCount = 9;
    [SerializeField] private Vector2 bloodSpeedRange = new Vector2(1.2f, 2.8f);
    [SerializeField] private Vector2 bloodSizeRange = new Vector2(0.04f, 0.1f);
    [SerializeField] private Vector2 bloodLifetimeRange = new Vector2(0.18f, 0.45f);
    [SerializeField] private float bloodUpwardBias = 0.35f;

    public void EmitMoveDust(float facingSign)
    {
        if (moveDust == null) return;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        float oppositeDirection = -Mathf.Sign(facingSign == 0f ? 1f : facingSign);

        for (int i = 0; i < moveDustCount; i++)
        {
            emitParams.velocity = new Vector3(
                oppositeDirection * Random.Range(moveDustSpeedRange.x, moveDustSpeedRange.y),
                Random.Range(moveDustUpRange.x, moveDustUpRange.y),
                0f
            );

            emitParams.startSize = Random.Range(moveDustSizeRange.x, moveDustSizeRange.y);
            emitParams.startLifetime = Random.Range(moveDustLifetimeRange.x, moveDustLifetimeRange.y);
            emitParams.rotation = Random.Range(0f, 360f);
            moveDust.Emit(emitParams, 1);
        }
    }

    public void EmitBloodParticles(Vector2 sourceWorldPosition, Vector2 enemyWorldPosition)
    {
        if (bloodParticles == null) return;

        Vector2 baseDirection = ((Vector2)enemyWorldPosition - sourceWorldPosition).normalized;
        if (baseDirection == Vector2.zero)
            baseDirection = Vector2.up;

        baseDirection.y = Mathf.Abs(baseDirection.y) + bloodUpwardBias;
        baseDirection.Normalize();

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        for (int i = 0; i < bloodCount; i++)
        {
            Vector2 spread = Random.insideUnitCircle * 0.55f;
            Vector2 direction = (baseDirection + spread).normalized;
            direction.y = Mathf.Abs(direction.y);

            emitParams.velocity = direction * Random.Range(bloodSpeedRange.x, bloodSpeedRange.y);
            emitParams.startSize = Random.Range(bloodSizeRange.x, bloodSizeRange.y);
            emitParams.startLifetime = Random.Range(bloodLifetimeRange.x, bloodLifetimeRange.y);
            emitParams.rotation = Random.Range(0f, 360f);
            bloodParticles.Emit(emitParams, 1);
        }
    }

    // Compatibility with older scripts that still call this method name.
    public void EmitDamageParticles()
    {
        EmitBloodParticles(transform.position + Vector3.left, transform.position);
    }

    // Compatibility with the previous EnemyVFX2D name.
    public void EmitDamageBurst(Vector2 sourceWorldPosition, Vector2 enemyWorldPosition)
    {
        EmitBloodParticles(sourceWorldPosition, enemyWorldPosition);
    }
}

using UnityEngine;

public class PlayerDamageParticles : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private ParticleSystem damageParticleSystem;

    [Header("Settings")]
    [SerializeField] private int particleCount = 4;
    [SerializeField] private float minSpeed = 0.8f;
    [SerializeField] private float maxSpeed = 1.6f;
    [SerializeField] private float minLifetime = 0.25f;
    [SerializeField] private float maxLifetime = 0.5f;
    [SerializeField] private float minSize = 0.04f;
    [SerializeField] private float maxSize = 0.09f;

    public void EmitDamageParticles()
    {
        if (damageParticleSystem == null) return;

        for (int i = 0; i < particleCount; i++)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

            Vector2 direction = Random.insideUnitCircle.normalized;

            // Push particles mostly upward
            direction.y = Mathf.Abs(direction.y);

            emitParams.velocity = direction * Random.Range(minSpeed, maxSpeed);
            emitParams.startLifetime = Random.Range(minLifetime, maxLifetime);
            emitParams.startSize = Random.Range(minSize, maxSize);
            emitParams.rotation = Random.Range(0f, 360f);

            damageParticleSystem.Emit(emitParams, 1);
        }
    }
}
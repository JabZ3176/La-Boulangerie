using UnityEngine;

public class Hazard : MonoBehaviour
{
    #region SETTINGS
    [Header("Hazard Type")]
    public HazardType hazardType;   // set to Fire or Spike in the Inspector

    public enum HazardType
    {
        Fire,
        Spike
    }

    [Header("Damage")]
    public float damageCooldown = 1f;   // seconds between each hit for fire
                                        // spikes deal damage instantly on contact
    #endregion

    #region PRIVATE VARIABLES
    private float lastDamageTime = -1f;
    #endregion

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DamagePlayer(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // fire keeps damaging while player stays in it
        // spikes only damage on first contact so skip stay for spikes
        if (hazardType == HazardType.Fire && other.CompareTag("Player"))
        {
            DamagePlayer(other.gameObject);
        }
    }
    #endregion

    #region DAMAGE
    private void DamagePlayer(GameObject playerObject)
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;

        // play the correct sound for this hazard type
        if (SoundManager.Instance != null)
        {
            if (hazardType == HazardType.Fire)
                SoundManager.Instance.PlayFireHurt();
            else if (hazardType == HazardType.Spike)
                SoundManager.Instance.PlaySpikeHit();
        }

        Player player = playerObject.GetComponent<Player>();
        if (player != null)
            player.TakeDamage();
    }
    #endregion
}
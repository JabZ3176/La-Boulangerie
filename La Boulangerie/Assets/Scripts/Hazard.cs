using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    #region SETTINGS
    [Header("Hazard Type")]
    public HazardType hazardType;

    public enum HazardType
    {
        Fire,
        Spike
    }

    [Header("Damage")]
    public int damageAmount = 1;
    public float damageCooldown = 0.8f;

    [Header("Ambient Sound")]
    public float ambientRange = 5f;         // how close player needs to be to hear fire
    public float ambientVolume = 0.4f;      // volume of the ambient fire sound
    public float ambientInterval = 1.5f;    // how often the ambient sound plays
    #endregion

    #region PRIVATE VARIABLES
    private float lastDamageTime = -999f;
    private float lastAmbientTime = -999f;
    private bool playerInside = false;
    private GameObject playerObject;
    private Transform playerTransform;
    #endregion

    #region START
    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

        // find player transform for distance check
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }
    #endregion

    #region UPDATE
    void Update()
    {
        // fire continuously damages while player is inside
        if (hazardType == HazardType.Fire && playerInside && playerObject != null)
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                DamagePlayer(playerObject);
            }
        }

        // play ambient fire sound when player is nearby
        if (hazardType == HazardType.Fire && playerTransform != null)
        {
            float distance = Vector2.Distance(
                transform.position,
                playerTransform.position
            );

            // volume fades based on distance
            float volume = Mathf.Lerp(ambientVolume, 0f, distance / ambientRange);

            if (distance <= ambientRange &&
    Time.time - lastAmbientTime >= ambientInterval)
            {
                lastAmbientTime = Time.time;

                if (SoundManager.Instance != null && volume > 0.05f)
                    SoundManager.Instance.PlayFireAmbient(volume);
            }
        }
    }
    #endregion

    #region TRIGGER EVENTS
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        playerObject = other.gameObject;

        DamagePlayer(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        playerObject = null;
    }
    #endregion

    #region DAMAGE
    private void DamagePlayer(GameObject target)
    {
        lastDamageTime = Time.time;

        if (SoundManager.Instance != null)
        {
            if (hazardType == HazardType.Fire)
                SoundManager.Instance.PlayFireHurt();
            else if (hazardType == HazardType.Spike)
                SoundManager.Instance.PlaySpikeHit();
        }

        Player player = target.GetComponent<Player>();
        if (player != null)
            player.TakeDamage();
    }
    #endregion
}
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
    #endregion

    #region PRIVATE VARIABLES
    private float lastDamageTime = -999f;
    private bool playerInside = false;
    private GameObject playerObject;
    #endregion

    #region START
    void Start()
    {
        // make sure this object has a trigger collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
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
    }
    #endregion

    #region TRIGGER EVENTS
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        playerObject = other.gameObject;

        // both fire and spike damage immediately on contact
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
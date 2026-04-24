using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // PATROL
    // ─────────────────────────────────────────────
    [Header("Patrol")]
    public float speed = 2f;        // how fast the enemy moves between patrol points
    public Transform[] points;      // the two (or more) patrol points in the scene

    // ─────────────────────────────────────────────
    // STATS
    // ─────────────────────────────────────────────
    [Header("Stats")]
    public int maxHealth = 3;       // how many hits the enemy can take
    private int currentHealth;      // tracks current health during gameplay
    private bool isDead = false;    // stops death from firing more than once

    // ─────────────────────────────────────────────
    // STUN
    // ─────────────────────────────────────────────
    [Header("Stun")]
    public float stunDuration = 2f; // how long the enemy is stunned after being slammed
    private bool isStunned = false; // tracks whether the enemy is currently stunned

    // ─────────────────────────────────────────────
    // DAMAGE
    // ─────────────────────────────────────────────
    [Header("Damage")]
    public int damageAmount = 1;        // how much damage the enemy deals to the player
    public float damageCooldown = 1f;   // seconds between each hit so it doesnt hit every frame
    private float lastDamageTime = -1f; // tracks when the enemy last dealt damage

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private int i;                          // current patrol point index
    private SpriteRenderer spriteRenderer;  // the enemy's sprite renderer

    // ─────────────────────────────────────────────
    // START — runs once when the scene loads
    // ─────────────────────────────────────────────
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth; // set health to full at the start
    }

    // ─────────────────────────────────────────────
    // UPDATE — runs every frame
    // ─────────────────────────────────────────────
    void Update()
    {
        // stop all behaviour if dead or stunned
        if (isDead || isStunned) return;

        Patrol();
    }

    // ─────────────────────────────────────────────
    // PATROL
    // ─────────────────────────────────────────────
    private void Patrol()
    {
        if (points.Length == 0) return; // safety check — needs at least one point

        // if close enough to the current point move to the next one
        if (Vector2.Distance(transform.position, points[i].position) < 0.25f)
        {
            i++;
            if (i == points.Length) i = 0; // loop back to the first point
        }

        // move towards the current patrol point
        transform.position = Vector2.MoveTowards(
            transform.position,
            points[i].position,
            speed * Time.deltaTime
        );

        // flip the sprite to face the direction of movement
        spriteRenderer.flipX = (transform.position.x - points[i].position.x) < 0f;
    }

    // ─────────────────────────────────────────────
    // PLAYER CONTACT — deals damage when touching player
    // ─────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        // when the player first touches the enemy
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // while the player stays in contact keep trying to deal damage
        // the cooldown inside DealDamageToPlayer prevents it hitting every frame
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other.gameObject);
        }
    }

    private void DealDamageToPlayer(GameObject playerObject)
    {
        // stunned enemies cannot hurt the player
        if (isStunned) return;

        // only deal damage if enough time has passed since the last hit
        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;

        // get the Player script and call TakeDamage on it
        Player player = playerObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }

    // ─────────────────────────────────────────────
    // STUN — called from Player.cs after a slam
    // ─────────────────────────────────────────────
    public void Stun()
    {
        if (isDead) return; // cant stun a dead enemy
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        spriteRenderer.color = Color.yellow; // flash yellow to show the enemy is stunned

        yield return new WaitForSeconds(stunDuration); // wait for stun to wear off

        isStunned = false;
        spriteRenderer.color = Color.white; // return to normal color
    }

    // ─────────────────────────────────────────────
    // TAKE DAMAGE — called from Player.cs after a slam
    // ─────────────────────────────────────────────
    public void TakeDamage(int damage)
    {
        if (isDead) return; // cant damage a dead enemy

        currentHealth -= damage;

        StartCoroutine(HitFlash()); // briefly flash red to show damage was dealt

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;   // flash red when hit
        yield return new WaitForSeconds(0.1f);

        // return to yellow if still stunned, otherwise return to normal
        spriteRenderer.color = isStunned ? Color.yellow : Color.white;
    }

    // ─────────────────────────────────────────────
    // DEATH
    // ─────────────────────────────────────────────
    private void Die()
    {
        isDead = true;
        StopAllCoroutines(); // stop any running flashes or stuns

        // destroy the enemy after a tiny delay so the hit flash has time to play
        Destroy(gameObject, 0.2f);
    }
}
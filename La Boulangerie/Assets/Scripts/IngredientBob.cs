using UnityEngine;

public class IngredientBob : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────
    [Header("Bob Settings")]
    public float bobHeight = 0.2f;  // how high the ingredient bobs
    public float bobSpeed = 2f;     // how fast it bobs

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private float startY;       // saves the starting Y position
    private float bobOffset;    // random offset so ingredients dont all bob in sync

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        // save the local Y position as the starting point
        startY = transform.localPosition.y;

        // give each ingredient a random offset so they dont all move together
        bobOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    // ─────────────────────────────────────────────
    // UPDATE
    // ─────────────────────────────────────────────
    void Update()
    {
        // calculate smooth up and down movement using a sine wave
        float newY = startY + Mathf.Sin(
            (Time.time * bobSpeed) + bobOffset) * bobHeight;

        // apply only the Y change keeping X and Z the same
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            newY,
            transform.localPosition.z
        );
    }
}
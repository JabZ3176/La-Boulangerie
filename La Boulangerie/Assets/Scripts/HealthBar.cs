using UnityEngine;
using UnityEngine.UI;

public class HeartHealthBar : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("Heart Images")]
    public Image heart1;    // drag Heart1 here in the Inspector
    public Image heart2;    // drag Heart2 here in the Inspector
    public Image heart3;    // drag Heart3 here in the Inspector

    [Header("Sprites")]
    public Sprite heartFull;    // drag Heart_Full.png here
    public Sprite heartEmpty;   // drag Heart_Empty.png here

    // ─────────────────────────────────────────────
    // UPDATE HEARTS — call this whenever health changes
    // ─────────────────────────────────────────────
    public void UpdateHearts(int currentHealth)
    {
        // update each heart based on the current health value
        // if health is 3 all full, if 2 one empty, if 1 two empty, if 0 all empty
        heart1.sprite = currentHealth >= 1 ? heartFull : heartEmpty;
        heart2.sprite = currentHealth >= 2 ? heartFull : heartEmpty;
        heart3.sprite = currentHealth >= 3 ? heartFull : heartEmpty;
    }
}
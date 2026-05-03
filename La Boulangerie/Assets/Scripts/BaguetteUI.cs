using UnityEngine;
using UnityEngine.UI;

public class BaguetteUI : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("Baguette Slot Icons")]
    public Image slot1;     // drag Baguette slot 1 image here
    public Image slot2;     // drag Baguette slot 2 image here
    public Image slot3;     // drag Baguette slot 3 image here

    [Header("Sprites")]
    public Sprite baguetteFull;     // drag your baguette sprite here
    public Sprite baguetteEmpty;    // drag your empty slot sprite here

    // ─────────────────────────────────────────────
    // UPDATE SLOTS — called whenever ammo changes
    // ─────────────────────────────────────────────
    public void UpdateSlots(int currentAmmo)
    {
        // update each slot based on how many baguettes the player has
        slot1.sprite = currentAmmo >= 1 ? baguetteFull : baguetteEmpty;
        slot2.sprite = currentAmmo >= 2 ? baguetteFull : baguetteEmpty;
        slot3.sprite = currentAmmo >= 3 ? baguetteFull : baguetteEmpty;
    }
}
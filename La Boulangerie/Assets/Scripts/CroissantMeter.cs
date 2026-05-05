using UnityEngine;
using UnityEngine.UI;

public class CroissantMeter : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("Croissant Slots")]
    public RectTransform slot1;     // leftmost slot — drains last
    public RectTransform slot2;     // middle slot
    public RectTransform slot3;     // rightmost slot — drains first

    [Header("Sprites")]
    public Sprite croissantFull;    // full croissant sprite
    public Sprite croissantEmpty;   // empty slot sprite

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private Image image1;           // image on slot 1
    private Image image2;           // image on slot 2
    private Image image3;           // image on slot 3

    private float slot1FullWidth;   // original full width of slot 1
    private float slot2FullWidth;   // original full width of slot 2
    private float slot3FullWidth;   // original full width of slot 3

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        image1 = slot1.GetComponent<Image>();
        image2 = slot2.GetComponent<Image>();
        image3 = slot3.GetComponent<Image>();

        // save the full width of each slot so we can shrink back to it
        slot1FullWidth = slot1.sizeDelta.x;
        slot2FullWidth = slot2.sizeDelta.x;
        slot3FullWidth = slot3.sizeDelta.x;

        // set image type to filled so it can shrink like a bar
        image1.type = Image.Type.Filled;
        image2.type = Image.Type.Filled;
        image3.type = Image.Type.Filled;

        // fill horizontally from left to right
        image1.fillMethod = Image.FillMethod.Horizontal;
        image2.fillMethod = Image.FillMethod.Horizontal;
        image3.fillMethod = Image.FillMethod.Horizontal;

        // fill from the left side
        image1.fillOrigin = (int)Image.OriginHorizontal.Left;
        image2.fillOrigin = (int)Image.OriginHorizontal.Left;
        image3.fillOrigin = (int)Image.OriginHorizontal.Left;

        // start full
        UpdateCroissants(1f, 1f);
    }

    // ─────────────────────────────────────────────
    // UPDATE CROISSANTS — called from Player.cs every frame
    // ─────────────────────────────────────────────
    public void UpdateCroissants(float currentEnergy, float maxEnergy)
    {
        // convert to a 0-1 percentage
        float energyPercent = Mathf.Clamp01(currentEnergy / maxEnergy);

        // split the percentage across 3 slots
        // each slot represents 1/3 of the total energy
        // slot3 drains first (top third), then slot2, then slot1

        float slot3Fill = Mathf.Clamp01(energyPercent * 3f);           // drains first: 100%-66%
        float slot2Fill = Mathf.Clamp01((energyPercent * 3f) - 1f);    // drains second: 66%-33%
        float slot1Fill = Mathf.Clamp01((energyPercent * 3f) - 2f);    // drains last: 33%-0%

        // apply fill amounts — this makes them slide like a bar
        image3.fillAmount = slot3Fill;
        image2.fillAmount = slot2Fill;
        image1.fillAmount = slot1Fill;

        // swap sprite to empty when fully drained
        image3.sprite = slot3Fill > 0 ? croissantFull : croissantEmpty;
        image2.sprite = slot2Fill > 0 ? croissantFull : croissantEmpty;
        image1.sprite = slot1Fill > 0 ? croissantFull : croissantEmpty;
    }
}
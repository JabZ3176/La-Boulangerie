using UnityEngine;
using UnityEngine.UI;

public class CroissantMeter : MonoBehaviour
{
    #region REFERENCES
    [Header("Croissant Slots")]
    public RectTransform slot1;
    public RectTransform slot2;
    public RectTransform slot3;

    [Header("Sprites")]
    public Sprite croissantFull;
    public Sprite croissantEmpty;
    #endregion

    #region PRIVATE VARIABLES
    private Image image1;
    private Image image2;
    private Image image3;

    private float slot1FullWidth;
    private float slot2FullWidth;
    private float slot3FullWidth;
    #endregion

    #region START
    void Start()
    {
        image1 = slot1.GetComponent<Image>();
        image2 = slot2.GetComponent<Image>();
        image3 = slot3.GetComponent<Image>();

        slot1FullWidth = slot1.sizeDelta.x;
        slot2FullWidth = slot2.sizeDelta.x;
        slot3FullWidth = slot3.sizeDelta.x;

        image1.type = Image.Type.Filled;
        image2.type = Image.Type.Filled;
        image3.type = Image.Type.Filled;

        image1.fillMethod = Image.FillMethod.Horizontal;
        image2.fillMethod = Image.FillMethod.Horizontal;
        image3.fillMethod = Image.FillMethod.Horizontal;

        image1.fillOrigin = (int)Image.OriginHorizontal.Left;
        image2.fillOrigin = (int)Image.OriginHorizontal.Left;
        image3.fillOrigin = (int)Image.OriginHorizontal.Left;

        UpdateCroissants(1f, 1f);
    }
    #endregion

    #region CROISSANTS
    public void UpdateCroissants(float currentEnergy, float maxEnergy)
    {
        if (image1 == null || image2 == null || image3 == null) return;

        float energyPercent = Mathf.Clamp01(currentEnergy / maxEnergy);

        float slot3Fill = Mathf.Clamp01(energyPercent * 3f);
        float slot2Fill = Mathf.Clamp01((energyPercent * 3f) - 1f);
        float slot1Fill = Mathf.Clamp01((energyPercent * 3f) - 2f);

        image3.fillAmount = slot3Fill;
        image2.fillAmount = slot2Fill;
        image1.fillAmount = slot1Fill;

        image3.sprite = slot3Fill > 0 ? croissantFull : croissantEmpty;
        image2.sprite = slot2Fill > 0 ? croissantFull : croissantEmpty;
        image1.sprite = slot1Fill > 0 ? croissantFull : croissantEmpty;
    }
    #endregion
}

using UnityEngine;
using UnityEngine.UI;

public class BaguetteUI : MonoBehaviour
{
    #region REFERENCES
    [Header("Baguette Slot Icons")]
    public Image slot1;
    public Image slot2;
    public Image slot3;

    [Header("Sprites")]
    public Sprite baguetteFull;
    public Sprite baguetteEmpty;
    #endregion

    #region UI UPDATES
    public void UpdateSlots(int currentAmmo)
    {
        slot1.sprite = currentAmmo >= 1 ? baguetteFull : baguetteEmpty;
        slot2.sprite = currentAmmo >= 2 ? baguetteFull : baguetteEmpty;
        slot3.sprite = currentAmmo >= 3 ? baguetteFull : baguetteEmpty;
    }
    #endregion
}

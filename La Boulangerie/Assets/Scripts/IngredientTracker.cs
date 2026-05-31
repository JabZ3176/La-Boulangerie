using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IngredientTracker : MonoBehaviour
{
    #region REFERENCES
    [Header("Text References")]
    public TextMeshProUGUI flourText;
    public TextMeshProUGUI milkText;
    public TextMeshProUGUI butterText;
    public TextMeshProUGUI totalText;

    [Header("Icons")]
    public Image flourIcon;
    public Image milkIcon;
    public Image butterIcon;
    #endregion

    #region ON ENABLE
    void OnEnable()
    {
        // refresh every time the panel is opened
        UpdateDisplay();
    }
    #endregion

    #region UPDATE DISPLAY
    public void UpdateDisplay()
    {
        int flour = PlayerPrefs.GetInt("TotalFlour", 0);
        int milk = PlayerPrefs.GetInt("TotalMilk", 0);
        int butter = PlayerPrefs.GetInt("TotalButter", 0);
        int total = flour + milk + butter;

        if (flourText != null)
            flourText.text = "Flour:     " + flour;

        if (milkText != null)
            milkText.text = "Milk:      " + milk;

        if (butterText != null)
            butterText.text = "Butter:    " + butter;

        if (totalText != null)
            totalText.text = "Total:     " + total;
    }
    #endregion
}
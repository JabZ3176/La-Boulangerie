using UnityEngine;
using UnityEngine.UI;

public class HeartHealthBar : MonoBehaviour
{
    #region REFERENCES
    [Header("Heart Images")]
    public Image heart1;
    public Image heart2;
    public Image heart3;

    [Header("Sprites")]
    public Sprite heartFull;
    public Sprite heartEmpty;
    #endregion

    #region HEARTS
    public void UpdateHearts(int currentHealth)
    {
        heart1.sprite = currentHealth >= 1 ? heartFull : heartEmpty;
        heart2.sprite = currentHealth >= 2 ? heartFull : heartEmpty;
        heart3.sprite = currentHealth >= 3 ? heartFull : heartEmpty;
    }
    #endregion
}

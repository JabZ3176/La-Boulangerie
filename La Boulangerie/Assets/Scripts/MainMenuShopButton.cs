using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuShopButton : MonoBehaviour
{
    #region SETTINGS
    [Header("Scene")]
    public string shopSceneName = "Shop";

    [Header("Optional UI")]
    public Button shopButton;
    public Graphic[] graphicsToFadeWhenLocked;
    public float lockedAlpha = 0.3f;
    #endregion

    #region START
    private void Start()
    {
        if (shopButton == null)
            shopButton = GetComponent<Button>();

        Refresh();
    }
    #endregion

    #region REFRESH
    public void Refresh()
    {
        bool unlocked = PlayerPrefs.GetInt("ShopUnlocked", 0) == 1;

        if (shopButton != null)
            shopButton.interactable = unlocked;

        if (graphicsToFadeWhenLocked != null)
        {
            for (int i = 0; i < graphicsToFadeWhenLocked.Length; i++)
            {
                if (graphicsToFadeWhenLocked[i] == null) continue;

                Color color = graphicsToFadeWhenLocked[i].color;
                color.a = unlocked ? 1f : lockedAlpha;
                graphicsToFadeWhenLocked[i].color = color;
            }
        }
    }
    #endregion

    #region BUTTON
    public void OpenShop()
    {
        if (PlayerPrefs.GetInt("ShopUnlocked", 0) != 1) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(shopSceneName);
    }
    #endregion
}

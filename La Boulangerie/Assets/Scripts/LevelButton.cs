using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    #region SETTINGS
    public int level;
    [Range(0f, 1f)] public float lockedAlpha = 0.3f;
    #endregion

    #region START
    void Start()
    {
        RefreshButtonState();
    }
    #endregion

    #region REFRESH
    public void RefreshButtonState()
    {
        Button btn = GetComponent<Button>();
        if (btn == null) return;

        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);
        bool unlocked = level <= levelReached;

        btn.interactable = unlocked;

        TextMeshProUGUI label = GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            Color color = label.color;
            color.a = unlocked ? 1f : lockedAlpha;
            label.color = color;
        }

        MenuButtonVisuals visuals = GetComponent<MenuButtonVisuals>();
        if (visuals != null)
            visuals.Refresh();
    }
    #endregion
}

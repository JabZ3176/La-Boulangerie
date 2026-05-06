using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    #region SETTINGS
    public int level;
    #endregion

    #region START
    void Start()
    {
        Button btn = GetComponent<Button>();

        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        if (level > levelReached)
        {
            btn.interactable = false;

            TextMeshProUGUI label = GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }
        else
        {
            btn.interactable = true;
        }
    }
    #endregion
}

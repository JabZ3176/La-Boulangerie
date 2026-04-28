using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int level; // which level this button leads to — set in the Inspector

    void Start()
    {
        Button btn = GetComponent<Button>();

        // get the highest level the player has reached (default is 1 if never saved)
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        // lock the button if the player hasnt reached this level yet
        if (level > levelReached)
        {
            btn.interactable = false;

            // optionally dim the button text to make it obvious its locked
            TextMeshProUGUI label = GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.color = new Color(1f, 1f, 1f, 0.3f); // make text faded
            }
        }
        else
        {
            btn.interactable = true;
        }
    }
}
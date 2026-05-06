using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    #region REFERENCES
    [Header("Panel")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI panelText;
    public Button continueButton;
    public RectTransform arrow;

    [Header("Next Scene")]
    public string nextSceneName = "Level1";
    #endregion

    #region ARROW SETTINGS
    [Header("Arrow Positions Per Panel")]
    public Vector2[] arrowPositions;
    public float[] arrowRotations;
    #endregion

    #region PANEL MESSAGES
    private string[] panelMessages = new string[]
    {
        // Panel 0 — Welcome
        "WELCOME TO LA BOULANGERIE!\n\n" +
        "You are Pierre, a brave baker on a mission to\n" +
        "recover stolen ingredients from across Paris!\n\n" +
        "This tutorial will teach you everything you need\n" +
        "to know before your adventure begins.\n\n" +
        "Press Continue to start learning!",

        // Panel 1 — Movement
        "CONTROLS\n\n" +
        "Move Left:       A or ← Arrow\n" +
        "Move Right:      D or → Arrow\n" +
        "Jump:            Space\n" +
        "Sprint:          Hold Shift\n" +
        "Slam:            S or ↓ Arrow (in air)\n" +
        "Throw Baguette:  Z",

        // Panel 2 — Ingredients
        "INGREDIENTS\n\n" +
        "Collect ingredients scattered around the level!\n" +
        "Each level requires a minimum amount of each\n" +
        "ingredient to unlock the door.\n\n" +
        "Walk into an ingredient to collect it.",

        // Panel 3 — Baguette
        "BAGUETTE THROWING\n\n" +
        "Find baguettes hidden in the level and pick them up.\n" +
        "You can carry up to 3 at a time.\n\n" +
        "Press Z to throw a baguette at enemies!\n" +
        "Throwing while jumping gives extra height.",

        // Panel 4 — Stomp
        "STOMPING ENEMIES\n\n" +
        "Jump above an enemy then press S or ↓ to slam down!\n" +
        "This stuns the enemy temporarily.\n\n" +
        "Chain 5 stomps in a row to activate a\n" +
        "temporary golden buff!",

        // Panel 5 — Door
        "THE DOOR\n\n" +
        "Collect enough ingredients to unlock the door!\n" +
        "The ingredient counter turns green when you\n" +
        "have collected enough of each type.\n\n" +
        "Walk through the door to reach the next level!\n" +
        "Good luck, baker!"
    };
    #endregion

    #region PRIVATE VARIABLES
    private int currentPanel = -1;
    private bool panelIsShowing = false;
    private Transform playerTransform;
    #endregion

    #region START
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        tutorialPanel.SetActive(false);
        continueButton.onClick.AddListener(OnContinuePressed);
        Time.timeScale = 1f;

        // automatically show the welcome panel when the scene loads
        // player doesnt need to move to see this
        ShowPanelByIndex(0);
    }
    #endregion

    #region SHOW PANEL
    public void ShowPanelByIndex(int index)
    {
        if (index <= currentPanel) return;

        currentPanel = index;
        panelIsShowing = true;

        Time.timeScale = 0f;
        tutorialPanel.SetActive(true);

        if (index < panelMessages.Length)
            panelText.text = panelMessages[index];

        if (arrow != null)
        {
            if (index < arrowPositions.Length)
                arrow.anchoredPosition = arrowPositions[index];

            if (index < arrowRotations.Length)
                arrow.localRotation = Quaternion.Euler(0f, 0f, arrowRotations[index]);

            // hide arrow on welcome panel since there is nothing to point at
            arrow.gameObject.SetActive(index != 0);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region CONTINUE
    public void OnContinuePressed()
    {
        panelIsShowing = false;
        tutorialPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // if this was the welcome panel automatically show controls next
        if (currentPanel == 0)
        {
            ShowPanelByIndex(1);
            return;
        }

        // if last panel load the next scene
        if (currentPanel >= panelMessages.Length - 1)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
    }
    #endregion
}
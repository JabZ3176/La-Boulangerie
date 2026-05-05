using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("Panel")]
    public GameObject tutorialPanel;        // the single panel
    public TextMeshProUGUI panelText;       // the text on the panel
    public Button continueButton;           // the continue button
    public RectTransform arrow;             // the single arrow that repositions

    [Header("Next Scene")]
    public string nextSceneName = "Level1";

    // ─────────────────────────────────────────────
    // ARROW POSITIONS — set these in the Inspector
    // one Vector2 per panel matching what it points at
    // ─────────────────────────────────────────────
    [Header("Arrow Positions Per Panel")]
    public Vector2[] arrowPositions;    // set in Inspector for each panel
    public float[] arrowRotations;      // rotation in degrees per panel
                                        // 0 = pointing right
                                        // 90 = pointing up
                                        // 180 = pointing left
                                        // 270 = pointing down

    // ─────────────────────────────────────────────
    // PANEL MESSAGES
    // ─────────────────────────────────────────────
    private string[] panelMessages = new string[]
    {
        // Panel 0 — Movement
        "CONTROLS\n\n" +
        "Move Left:       A or ← Arrow\n" +
        "Move Right:      D or → Arrow\n" +
        "Jump:            Space\n" +
        "Sprint:          Hold Shift\n" +
        "Slam:            S or ↓ Arrow (in air)\n" +
        "Throw Baguette:  Z",

        // Panel 1 — Ingredients
        "INGREDIENTS\n\n" +
        "Collect ingredients scattered around the level!\n" +
        "Each level requires a minimum amount of each\n" +
        "ingredient to unlock the door.\n\n" +
        "Walk into an ingredient to collect it.",

        // Panel 2 — Baguette
        "BAGUETTE THROWING\n\n" +
        "Find baguettes hidden in the level and pick them up.\n" +
        "You can carry up to 3 at a time.\n\n" +
        "Press Z to throw a baguette at enemies!\n" +
        "Throwing while jumping gives extra height.",

        // Panel 3 — Stomp
        "STOMPING ENEMIES\n\n" +
        "Jump above an enemy then press S or ↓ to slam down!\n" +
        "This stuns the enemy temporarily.\n\n" +
        "Chain 5 stomps in a row to activate a\n" +
        "temporary golden buff!",

        // Panel 4 — Door
        "THE DOOR\n\n" +
        "Collect enough ingredients to unlock the door!\n" +
        "The ingredient counter turns green when you\n" +
        "have collected enough of each type.\n\n" +
        "Walk through the door to reach the next level!\n" +
        "Good luck, baker!"
    };

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private int currentPanel = -1;
    private bool panelIsShowing = false;
    private Transform playerTransform;

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // hide the panel at start
        tutorialPanel.SetActive(false);

        // hook up continue button
        continueButton.onClick.AddListener(OnContinuePressed);

        Time.timeScale = 1f;
    }

    // ─────────────────────────────────────────────
    // SHOW PANEL — called from TutorialTrigger.cs
    // ─────────────────────────────────────────────
    public void ShowPanelByIndex(int index)
    {
        // dont show the same panel twice
        if (index <= currentPanel) return;

        currentPanel = index;
        panelIsShowing = true;

        // pause the game
        Time.timeScale = 0f;

        // show the panel
        tutorialPanel.SetActive(true);

        // update the text
        if (index < panelMessages.Length)
            panelText.text = panelMessages[index];

        // reposition and rotate the arrow for this panel
        if (arrow != null)
        {
            // move arrow to the correct position for this panel
            if (index < arrowPositions.Length)
                arrow.anchoredPosition = arrowPositions[index];

            // rotate arrow to point at the correct element
            if (index < arrowRotations.Length)
                arrow.localRotation = Quaternion.Euler(0f, 0f, arrowRotations[index]);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ─────────────────────────────────────────────
    // ON CONTINUE PRESSED
    // ─────────────────────────────────────────────
    public void OnContinuePressed()
    {
        panelIsShowing = false;
        tutorialPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // if last panel load the next scene
        if (currentPanel >= panelMessages.Length - 1)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
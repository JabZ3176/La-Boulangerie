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

    #region ARROW POSITIONS
    [Header("Arrow Positions Per Panel")]
    public Vector2[] arrowPositions;
    public float[] arrowRotations;
    #endregion

    #region PANEL MESSAGES
    private string[] panelMessages = new string[]
    {
        "CONTROLS\n\n" +
        "Move Left:       'A' or ← Arrow\n" +
        "Move Right:      'D' or → Arrow\n" +
        "Jump:            'Space'\n" +
        "Sprint:          Hold 'Shift'\n" +
        "Slam:            'S' or ↓ Arrow (in air)\n" +
        "Throw Baguette:  'Z'",
        "INGREDIENTS\n\n" +
        "Collect ingredients scattered around the level!\n" +
        "Each level requires a minimum amount of each\n" +
        "ingredient to unlock the door.\n\n" +
        "Walk into an ingredient to collect it.",

        "BAGUETTE THROWING\n\n" +
        "Find baguettes hidden in the level and pick them up.\n" +
        "You can carry up to 3 at a time.\n\n" +
        "Press Z to throw a baguette at enemies!\n" +
        "Throwing while jumping gives extra height.",

        "STOMPING ENEMIES\n\n" +
        "Jump above an enemy then press S or ↓ to slam down!\n" +
        "This stuns the enemy temporarily.\n\n" +
        "Chain 5 stomps in a row to activate a\n" +
        "temporary golden buff!",

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
    }
    #endregion

    #region PANEL CONTROL
    public void ShowPanelByIndex(int index)
    {
        if (index <= currentPanel) return;

        currentPanel = index;

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
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnContinuePressed()
    {
        tutorialPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentPanel >= panelMessages.Length - 1)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
    }
    #endregion
}

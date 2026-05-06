using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    #region SETTINGS
    [Header("Settings")]
    public int panelIndex;
    #endregion

    #region PRIVATE VARIABLES
    private bool hasTriggered = false;
    private TutorialManager tutorialManager;
    #endregion

    #region START
    void Start()
    {
        tutorialManager = Object.FindFirstObjectByType<TutorialManager>();
    }
    #endregion

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            tutorialManager.ShowPanelByIndex(panelIndex);
        }
    }
    #endregion
}

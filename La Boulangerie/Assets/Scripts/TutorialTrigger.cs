using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────
    [Header("Settings")]
    public int panelIndex;  // which panel to show (0 = movement, 1 = ingredient etc)

    private bool hasTriggered = false;  // stops it triggering twice
    private TutorialManager tutorialManager;

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
    }

    // ─────────────────────────────────────────────
    // TRIGGER — fires when player enters the zone
    // ─────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            tutorialManager.ShowPanelByIndex(panelIndex);
        }
    }
}
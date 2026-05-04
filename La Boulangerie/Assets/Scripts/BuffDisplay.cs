using UnityEngine;
using TMPro;
using System.Collections;

public class BuffDisplay : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("UI References")]
    public TextMeshProUGUI buffText;     // drag your buff text here
    public GameObject buffPanel;         // drag the panel behind the text here

    // ─────────────────────────────────────────────
    // PRIVATE VARIABLES
    // ─────────────────────────────────────────────
    private Coroutine timerCoroutine;   // reference to the running timer

    // ─────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────
    void Start()
    {
        // hide the buff panel at the start
        if (buffPanel != null)
            buffPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // SHOW BUFF — called from Player.cs when buff starts
    // ─────────────────────────────────────────────
    public void ShowBuff(float duration)
    {
        // stop any existing timer before starting a new one
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        buffPanel.SetActive(true);
        timerCoroutine = StartCoroutine(BuffTimer(duration));
    }

    // ─────────────────────────────────────────────
    // HIDE BUFF — called when buff ends
    // ─────────────────────────────────────────────
    public void HideBuff()
    {
        if (buffPanel != null)
            buffPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // BUFF TIMER — counts down and updates the text
    // ─────────────────────────────────────────────
    private IEnumerator BuffTimer(float duration)
    {
        float timeLeft = duration;

        while (timeLeft > 0)
        {
            // update the text every frame with the countdown
            buffText.text = "5 Stomps! Buff Active: " + Mathf.CeilToInt(timeLeft) + "s";

            // change text color to red in the last 5 seconds as a warning
            buffText.color = timeLeft <= 5f ? Color.red : Color.yellow;

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        // hide the panel when the timer runs out
        HideBuff();
    }
}
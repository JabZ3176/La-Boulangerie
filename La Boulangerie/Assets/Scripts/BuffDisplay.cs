using System.Collections;
using TMPro;
using UnityEngine;

public class BuffDisplay : MonoBehaviour
{
    #region REFERENCES
    [Header("UI References")]
    public TextMeshProUGUI buffText;
    public GameObject buffPanel;
    #endregion

    #region PRIVATE VARIABLES
    private Coroutine timerCoroutine;
    #endregion

    #region START
    void Start()
    {
        if (buffPanel != null)
            buffPanel.SetActive(false);
    }
    #endregion

    #region BUFF DISPLAY
    public void ShowBuff(float duration)
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        buffPanel.SetActive(true);
        timerCoroutine = StartCoroutine(BuffTimer(duration));
    }

    public void HideBuff()
    {
        if (buffPanel != null)
            buffPanel.SetActive(false);
    }
    #endregion

    #region BUFF TIMER
    private IEnumerator BuffTimer(float duration)
    {
        float timeLeft = duration;

        while (timeLeft > 0)
        {
            buffText.text = "5 Stomps! Buff Active: " + Mathf.CeilToInt(timeLeft) + "s";
            buffText.color = timeLeft <= 5f ? Color.red : Color.yellow;

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        HideBuff();
    }
    #endregion
}

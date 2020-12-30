using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class MatchUI : MonoBehaviour
{
    private float timeLeft;
    [SerializeField] private TMP_Text leftTeamScoreText;
    [SerializeField] private TMP_Text rightTeamScoreText;
    [SerializeField] private TMP_Text leftTeamWinsText;
    [SerializeField] private TMP_Text rightTeamWinsText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject exitPanel;

    private void Awake()
    {
        MatchTracker.ClientMatchEnded += HandleMatchEnded;
        ScoreTracker.ClientScoreUpdated += HandleScoreUpdated;
        RoundTracker.ClientCountdownStarted += HandleCountdownStarted;
    }

    private void OnDestroy()
    {
        MatchTracker.ClientMatchEnded -= HandleMatchEnded;
        ScoreTracker.ClientScoreUpdated -= HandleScoreUpdated;
        RoundTracker.ClientCountdownStarted -= HandleCountdownStarted;
    }

    private void Update()
    {
        if (timeLeft < 0f) { return; }
        Countdown();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            exitPanel.SetActive(!exitPanel.activeSelf);
        }
    }

    private void Countdown()
    {
        timeLeft -= Time.deltaTime;
        float timeLeftRounded = ((int)(timeLeft * 10f)) / 10f;

        if (timeLeft > 0f)
            countdownText.text = timeLeftRounded.ToString();
        else
            countdownText.gameObject.SetActive(false);
    }

    private void HandleMatchEnded(bool isLeftTeamWin)
    {
        if (isLeftTeamWin)
            leftTeamWinsText.gameObject.SetActive(true);
        else
            rightTeamWinsText.gameObject.SetActive(true);
    }

    private void HandleScoreUpdated(int leftTeamScore, int rightTeamScore)
    {
        leftTeamScoreText.text = leftTeamScore.ToString();
        rightTeamScoreText.text = rightTeamScore.ToString();
    }

    private void HandleCountdownStarted(float timeBetweenRounds)
    {
        timeLeft = timeBetweenRounds;
        countdownText.text = timeBetweenRounds.ToString();
        countdownText.gameObject.SetActive(true);
    }

}

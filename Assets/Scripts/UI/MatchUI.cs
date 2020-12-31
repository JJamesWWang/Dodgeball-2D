using UnityEngine;
using TMPro;

public class MatchUI : MonoBehaviour
{
    private float timeLeft;
    [SerializeField] private TMP_Text leftTeamScoreText;
    [SerializeField] private TMP_Text rightTeamScoreText;
    [SerializeField] private TMP_Text countdownText;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        if (timeLeft < 0f) { return; }
        Countdown();
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

    private void SubscribeEvents()
    {
        RoundTracker.ClientCountdownStarted += HandleCountdownStarted;
        ScoreTracker.ClientScoreUpdated += HandleScoreUpdated;
    }

    private void HandleCountdownStarted(float timeBetweenRounds)
    {
        timeLeft = timeBetweenRounds;
        countdownText.text = timeBetweenRounds.ToString();
        countdownText.gameObject.SetActive(true);
    }

    private void HandleScoreUpdated(int leftTeamScore, int rightTeamScore)
    {
        leftTeamScoreText.text = leftTeamScore.ToString();
        rightTeamScoreText.text = rightTeamScore.ToString();
    }

    private void UnsubscribeEvents()
    {
        RoundTracker.ClientCountdownStarted -= HandleCountdownStarted;
        ScoreTracker.ClientScoreUpdated -= HandleScoreUpdated;
    }

}

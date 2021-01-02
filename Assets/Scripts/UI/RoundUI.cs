using UnityEngine;
using TMPro;

public class RoundUI : MonoBehaviour
{
    private float timeLeft;
    [SerializeField] private TMP_Text leftTeamScoreText;
    [SerializeField] private TMP_Text rightTeamScoreText;
    [SerializeField] private TMP_Text waitingForPlayersText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text leftTeamWinsText;
    [SerializeField] private TMP_Text rightTeamWinsText;

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
        MatchTracker.ClientMatchStarted += HandleMatchStarted;
        RoundTracker.ClientCountdownStarted += HandleCountdownStarted;
        ScoreTracker.ClientScoreUpdated += HandleScoreUpdated;
        RoundTracker.ClientRoundOver += HandleRoundOver;
    }

    private void HandleMatchStarted()
    {
        waitingForPlayersText.gameObject.SetActive(false);
    }

    private void HandleCountdownStarted(float timeBetweenRounds)
    {
        timeLeft = timeBetweenRounds;
        countdownText.text = timeBetweenRounds.ToString();
        countdownText.gameObject.SetActive(true);
        leftTeamWinsText.gameObject.SetActive(false);
        rightTeamWinsText.gameObject.SetActive(false);
    }

    private void HandleScoreUpdated(int leftTeamScore, int rightTeamScore)
    {
        leftTeamScoreText.text = leftTeamScore.ToString();
        rightTeamScoreText.text = rightTeamScore.ToString();
    }

    private void HandleRoundOver(bool isLeftTeamWin)
    {
        if (isLeftTeamWin)
            leftTeamWinsText.gameObject.SetActive(true);
        else
            rightTeamWinsText.gameObject.SetActive(true);
    }

    private void UnsubscribeEvents()
    {
        MatchTracker.ClientMatchStarted -= HandleMatchStarted;
        RoundTracker.ClientCountdownStarted -= HandleCountdownStarted;
        ScoreTracker.ClientScoreUpdated -= HandleScoreUpdated;
        RoundTracker.ClientRoundOver -= HandleRoundOver;
    }

}

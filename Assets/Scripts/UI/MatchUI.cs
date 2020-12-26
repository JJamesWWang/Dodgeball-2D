using UnityEngine;
using TMPro;

public class MatchUI : MonoBehaviour
{
    private float timeLeft;
    [SerializeField] private GameObject matchUIParent;
    [SerializeField] private TMP_Text leftTeamScoreText;
    [SerializeField] private TMP_Text rightTeamScoreText;
    [SerializeField] private TMP_Text leftTeamWinsText;
    [SerializeField] private TMP_Text rightTeamWinsText;
    [SerializeField] private TMP_Text countdownText;

    private void Start()
    {
        MatchTracker.ClientMatchStarted += HandleMatchStarted;
        MatchTracker.ClientMatchEnded += HandleMatchEnded;
        ScoreTracker.ClientScoreUpdated += HandleScoreUpdated;
        RoundTracker.ClientCountdownStarted += HandleCountdownStarted;
    }

    private void OnDestroy()
    {
        MatchTracker.ClientMatchStarted -= HandleMatchStarted;
        MatchTracker.ClientMatchEnded -= HandleMatchEnded;
        ScoreTracker.ClientScoreUpdated -= HandleScoreUpdated;
        RoundTracker.ClientCountdownStarted -= HandleCountdownStarted;
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

    private void HandleMatchStarted()
    {
        leftTeamWinsText.gameObject.SetActive(false);
        rightTeamWinsText.gameObject.SetActive(false);
        matchUIParent.gameObject.SetActive(true);
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

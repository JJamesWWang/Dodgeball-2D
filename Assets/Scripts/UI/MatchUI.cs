using UnityEngine;
using TMPro;

public class MatchUI : MonoBehaviour
{
    [SerializeField] private GameObject matchUIParent;
    [SerializeField] private TMP_Text leftTeamScoreText;
    [SerializeField] private TMP_Text rightTeamScoreText;
    [SerializeField] private TMP_Text leftTeamWinsText;
    [SerializeField] private TMP_Text rightTeamWinsText;

    private void Start()
    {
        MatchTracker.ClientMatchStarted += HandleMatchStarted;
        MatchTracker.ClientMatchEnded += HandleMatchEnded;
        ScoreTracker.ClientScoreUpdated += HandleScoreUpdated;
    }

    private void OnDestroy()
    {
        MatchTracker.ClientMatchStarted -= HandleMatchStarted;
        MatchTracker.ClientMatchEnded -= HandleMatchEnded;
        ScoreTracker.ClientScoreUpdated -= HandleScoreUpdated;
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
}

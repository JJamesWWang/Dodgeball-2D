using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MatchTracker : NetworkBehaviour
{
    [SerializeField] private Button startButton = null;
    private RoundTracker roundTracker;
    private PlayerTracker playerTracker;
    private ScoreTracker scoreTracker;

    private void Start()
    {
        roundTracker = GetComponentInParent<RoundTracker>();
        playerTracker = GetComponentInParent<PlayerTracker>();
        scoreTracker = GetComponentInParent<ScoreTracker>();
        if (!NetworkServer.active)
            startButton.gameObject.SetActive(false);
    }

    public void StartMatch()
    {
        scoreTracker.ResetScore();
        startButton.gameObject.SetActive(false);
        roundTracker.StartRound();
    }

    public void ResetMatch()
    {
        playerTracker.DespawnPlayers();
        startButton.gameObject.SetActive(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        RoundTracker.ServerRoundEnded += HandleRoundEnded;
        ScoreTracker.ServerScoreUpdated += HandleScoreUpdated;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        RoundTracker.ServerRoundEnded -= HandleRoundEnded;
        ScoreTracker.ServerScoreUpdated -= HandleScoreUpdated;
    }

    [Server]
    private void HandleRoundEnded(bool isLeftTeamWin)
    {
        IncrementScore(isLeftTeamWin);
        roundTracker.StartRound();
    }

    [Server]
    private void IncrementScore(bool isLeftTeamWin)
    {
        if (isLeftTeamWin)
            scoreTracker.IncrementLeftTeamScore();
        else
            scoreTracker.IncrementRightTeamScore();
    }

    [Server]
    private void HandleScoreUpdated(int leftTeamScore, int rightTeamScore)
    {
        CheckMatchOver(leftTeamScore, rightTeamScore);
    }

    [Server]
    private void CheckMatchOver(int leftTeamScore, int rightTeamScore)
    {
        int scoreToWin = scoreTracker.ScoreToWin;
        bool matchEnded = false;
        if (leftTeamScore == scoreToWin && rightTeamScore == scoreToWin)
        {
            Debug.Log("Tie!");
            matchEnded = true;
        }
        else if (leftTeamScore == scoreToWin)
        {
            Debug.Log("Left Team Wins!");
            matchEnded = true;
        }
        else if (rightTeamScore == scoreToWin)
        {
            Debug.Log("Right Team Wins!");
            matchEnded = true;
        }

        if (matchEnded)
            ResetMatch();
    }

}

using UnityEngine;
using Mirror;
using System;

public class MatchTracker : NetworkBehaviour
{
    private RoundTracker roundTracker;
    private PlayerTracker playerTracker;
    private ScoreTracker scoreTracker;

    public static event Action ClientMatchStarted;
    public static event Action<bool> ClientMatchEnded;

    private void Start()
    {
        roundTracker = GetComponent<RoundTracker>();
        playerTracker = GetComponent<PlayerTracker>();
        scoreTracker = GetComponent<ScoreTracker>();
    }

    #region Server

    [Server]
    public void StartMatch()
    {
        scoreTracker.ResetScore();
        roundTracker.StartRound();
        InvokeMatchStarted();
    }

    [Server]
    public void ResetMatch()
    {
        playerTracker.DespawnPlayers();
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
        if (leftTeamScore != scoreToWin && rightTeamScore != scoreToWin) { return; }

        bool isLeftTeamWin = leftTeamScore == scoreToWin;
        InvokeMatchEnded(isLeftTeamWin);
        ResetMatch();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void InvokeMatchStarted()
    {
        ClientMatchStarted?.Invoke();
    }

    [ClientRpc]
    private void InvokeMatchEnded(bool isLeftTeamWin)
    {
        ClientMatchEnded?.Invoke(isLeftTeamWin);
    }

    #endregion

}

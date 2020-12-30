using UnityEngine;
using Mirror;
using System;

// Events: ServerMatchStarted, ServerMatchEnded, ClientMatchEnded
// Methods: [Server] StartMatch, [Server] ResetMatch, 
public class MatchTracker : NetworkBehaviour
{
    private RoundTracker roundTracker;
    private PlayerTracker playerTracker;
    private ScoreTracker scoreTracker;

    public static event Action ServerMatchStarted;
    public static event Action ServerMatchEnded;
    /// <summary> bool: isLeftTeamWin </summary>
    public static event Action<bool> ClientMatchEnded;

    private void Awake()
    {
        roundTracker = GetComponent<RoundTracker>();
        playerTracker = GetComponent<PlayerTracker>();
        scoreTracker = GetComponent<ScoreTracker>();
    }

    #region Server

    public override void OnStartServer()
    {
        SubscribeEvents();
    }
    public override void OnStopServer()
    {
        UnsubscribeEvents();
    }

    [Server]
    public void StartMatch()
    {
        scoreTracker.ResetScore();
        roundTracker.StartRound();
        ServerMatchStarted?.Invoke();
    }

    [Server]
    public void ResetMatch()
    {
        playerTracker.DespawnPlayers();
    }

    [Server]
    private void SubscribeEvents()
    {
        RoundTracker.ServerRoundOver += HandleRoundEnded;
        ScoreTracker.ServerScoreUpdated += HandleScoreUpdated;
    }

    [Server]
    private void HandleRoundEnded(bool isLeftTeamWin)
    {
        IncrementScore(isLeftTeamWin);
        if (GameState.Instance.IsInPlay)
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
        if (!IsMatchOver(leftTeamScore, rightTeamScore)) { return; }
        ServerMatchEnded?.Invoke();
        InvokeMatchEnded(IsLeftTeamWin(leftTeamScore, rightTeamScore));
        ResetMatch();
    }

    [Server]
    private bool IsMatchOver(int leftTeamScore, int rightTeamScore)
    {
        int scoreToWin = scoreTracker.ScoreToWin;
        return leftTeamScore == scoreToWin || rightTeamScore == scoreToWin;
    }

    [Server]
    private bool IsLeftTeamWin(int leftTeamScore, int rightTeamScore)
    {
        int scoreToWin = scoreTracker.ScoreToWin;
        return leftTeamScore == scoreToWin;
    }

    [Server]
    private void UnsubscribeEvents()
    {
        RoundTracker.ServerRoundOver -= HandleRoundEnded;
        ScoreTracker.ServerScoreUpdated -= HandleScoreUpdated;
    }

    #endregion

    #region Client

    [ClientRpc]
    private void InvokeMatchEnded(bool isLeftTeamWin)
    {
        ClientMatchEnded?.Invoke(isLeftTeamWin);
    }

    #endregion

}

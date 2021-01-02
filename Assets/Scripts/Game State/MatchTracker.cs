using UnityEngine;
using Mirror;
using System;

// Events: ServerMatchStarted, ServerMatchEnded, ClientMatchStarted, ClientMatchEnded
// Methods: [Server] StartMatch, [Server] ResetMatch, [Server] EndMatch
public class MatchTracker : NetworkBehaviour
{
    private bool isOver;
    private RoundTracker roundTracker;
    private PlayerTracker playerTracker;
    private ScoreTracker scoreTracker;

    public static event Action ServerMatchStarted;
    public static event Action ServerMatchEnded;
    public static event Action ClientMatchStarted;
    /// <summary> bool: isLeftTeamWin </summary>
    public static event Action<bool> ClientMatchEnded;

    private void Awake()
    {
        roundTracker = GetComponent<RoundTracker>();
        playerTracker = GetComponent<PlayerTracker>();
        scoreTracker = GetComponent<ScoreTracker>();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    #region Server

    [Server]
    public void StartMatch()
    {
        scoreTracker.ResetScore();
        roundTracker.StartRound();
        RpcInvokeMatchStarted();
        ServerMatchStarted?.Invoke();
    }

    [Server]
    public void ResetMatch()
    {
        playerTracker.DespawnPlayers();
    }

    [Server]
    public void EndMatch(bool isLeftTeamWin)
    {
        ResetMatch();
        RpcInvokeMatchEnded(isLeftTeamWin);
        ServerMatchEnded?.Invoke();
    }

    [ServerCallback]
    private void SubscribeEvents()
    {
        RoundTracker.ServerRoundOver += HandleRoundOver;
        ScoreTracker.ServerScoreUpdated += HandleScoreUpdated;
    }

    [Server]
    private void HandleRoundOver(bool isLeftTeamWin)
    {
        IncrementScore(isLeftTeamWin);
        if (!isOver)
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
        isOver = true;
        EndMatch(IsLeftTeamWin(leftTeamScore, rightTeamScore));
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

    [ServerCallback]
    private void UnsubscribeEvents()
    {
        RoundTracker.ServerRoundOver -= HandleRoundOver;
        ScoreTracker.ServerScoreUpdated -= HandleScoreUpdated;
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcInvokeMatchStarted()
    {
        ClientMatchStarted?.Invoke();
    }

    [ClientRpc]
    private void RpcInvokeMatchEnded(bool isLeftTeamWin)
    {
        ClientMatchEnded?.Invoke(isLeftTeamWin);
    }

    #endregion

}

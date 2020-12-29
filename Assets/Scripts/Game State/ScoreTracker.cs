using UnityEngine;
using Mirror;
using System;

public class ScoreTracker : NetworkBehaviour
{
    [SerializeField] private int scoreToWin = 11;

    [SyncVar(hook = nameof(HandleScoreUpdated))]
    private int leftTeamScore = 0;
    [SyncVar(hook = nameof(HandleScoreUpdated))]
    private int rightTeamScore = 0;

    public int ScoreToWin { get { return scoreToWin; } }
    public int LeftTeamScore { get { return leftTeamScore; } }
    public int RightTeamScore { get { return rightTeamScore; } }

    public static event Action<int, int> ServerScoreUpdated;
    public static event Action<int, int> ClientScoreUpdated;

    private void HandleScoreUpdated(int _oldScore, int _newScore)
    {
        ServerScoreUpdated?.Invoke(leftTeamScore, rightTeamScore);
        ClientScoreUpdated?.Invoke(leftTeamScore, rightTeamScore);
    }

    #region Server

    [Server]
    public void ResetScore()
    {
        leftTeamScore = 0;
        rightTeamScore = 0;
    }

    [Server]
    public void IncrementLeftTeamScore()
    {
        leftTeamScore += 1;
    }

    [Server]
    public void IncrementRightTeamScore()
    {
        rightTeamScore += 1;
    }

    #endregion
}

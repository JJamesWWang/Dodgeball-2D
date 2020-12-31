using UnityEngine;
using Mirror;
using System;

// Properties: ScoreToWin, LeftTeamScore, RightTeamScore
// Events: ServerScoreUpdated, ClientScoreUpdated
// Methods: [Server] ResetScore, [Server] IncrementLeftTeamScore, [Server] IncrementRightTeamScore
public class ScoreTracker : NetworkBehaviour
{
    [SerializeField] private int scoreToWin = 11;

    // Temporarily serialized for debugging purposes
    [SyncVar(hook = nameof(HandleScoreUpdated))]
    [SerializeField] private int leftTeamScore = 0;
    [SyncVar(hook = nameof(HandleScoreUpdated))]
    [SerializeField] private int rightTeamScore = 0;

    public int ScoreToWin { get { return scoreToWin; } }
    public int LeftTeamScore { get { return leftTeamScore; } }
    public int RightTeamScore { get { return rightTeamScore; } }

    /// <summary> int: leftTeamScore, int: rightTeamScore </summary>
    public static event Action<int, int> ServerScoreUpdated;
    /// <summary> int: leftTeamScore, int: rightTeamScore </summary>
    public static event Action<int, int> ClientScoreUpdated;


    #region Server

    [Server]
    public void ResetScore()
    {
        leftTeamScore = 0;
        rightTeamScore = 0;
        ServerScoreUpdated?.Invoke(leftTeamScore, rightTeamScore);
    }

    [Server]
    public void IncrementLeftTeamScore()
    {
        leftTeamScore += 1;
        ServerScoreUpdated?.Invoke(leftTeamScore, rightTeamScore);
    }

    [Server]
    public void IncrementRightTeamScore()
    {
        rightTeamScore += 1;
        ServerScoreUpdated?.Invoke(leftTeamScore, rightTeamScore);
    }

    #endregion

    #region Client

    [Client]
    private void HandleScoreUpdated(int _oldScore, int _newScore)
    {
        ClientScoreUpdated?.Invoke(leftTeamScore, rightTeamScore);
    }

    #endregion
}

using System.Collections;
using UnityEngine;
using Mirror;
using System;

public class GameState : NetworkBehaviour
{
    private MatchTracker matchTracker;
    private Room room;

    public static GameState Instance { get; private set; }
    public bool IsInPlay { get; private set; }
    public static Action ServerGameStateReady;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        matchTracker = GetComponent<MatchTracker>();
    }

    private void Start()
    {
        room = (Room)NetworkManager.singleton;
        ServerGameStateReady?.Invoke();
    }

    #region Server

    public override void OnStartServer()
    {
        MatchTracker.ServerMatchStarted += HandleMatchStarted;
        MatchTracker.ServerMatchEnded += HandleMatchEnded;
    }

    public override void OnStopServer()
    {
        MatchTracker.ServerMatchStarted -= HandleMatchStarted;
        MatchTracker.ServerMatchEnded -= HandleMatchEnded;
    }

    [Server]
    private void HandleMatchStarted()
    {
        IsInPlay = true;
    }

    [Server]
    private void HandleMatchEnded()
    {
        IsInPlay = false;
    }

    [Server]
    public void StartGame()
    {
        // Temporarily disabled for easier debugging
        //if (IsValidTeamComposition())
        matchTracker.StartMatch();
    }

    [Server]
    public void EndGame()
    {

    }

    [Server]
    private bool IsValidTeamComposition()
    {
        int leftTeamPlayerCount = 0;
        int rightTeamPlayerCount = 0;
        foreach (Connection connection in room.Connections)
        {
            var playerData = connection.GetComponent<PlayerData>();
            if (playerData.IsLeftTeam)
                leftTeamPlayerCount += 1;
            else
                rightTeamPlayerCount += 1;

        }

        return leftTeamPlayerCount > 0 && rightTeamPlayerCount > 0;
    }

    #endregion
}

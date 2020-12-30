using UnityEngine;
using Mirror;
using System;

// Properties: static Instance, IsInPlay 
// Events: ServerGameStateReady
// Methods: [Server] StartGame
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
        SubscribeEvents();
    }

    public override void OnStopServer()
    {
        UnsubscribeEvents();
    }

    [Server]
    public void StartGame()
    {
        // Temporarily disabled for easier debugging
        //if (IsValidTeamComposition())
        matchTracker.StartMatch();
    }

    [Server]
    private bool IsValidTeamComposition()
    {
        int leftTeamPlayerCount = 0;
        int rightTeamPlayerCount = 0;
        foreach (Player player in room.Players)
            if (player.IsLeftTeam)
                leftTeamPlayerCount += 1;
            else
                rightTeamPlayerCount += 1;
        return leftTeamPlayerCount > 0 && rightTeamPlayerCount > 0;
    }

    private void SubscribeEvents()
    {
        MatchTracker.ServerMatchStarted += HandleMatchStarted;
        MatchTracker.ServerMatchEnded += HandleMatchEnded;
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

    private void UnsubscribeEvents()
    {
        MatchTracker.ServerMatchStarted -= HandleMatchStarted;
        MatchTracker.ServerMatchEnded -= HandleMatchEnded;
    }

    #endregion
}

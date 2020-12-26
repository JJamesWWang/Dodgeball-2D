using UnityEngine;
using Mirror;

public class GameState : NetworkBehaviour
{
    private MatchTracker matchTracker;
    private DodgeballNetworkManager dodgeballNetworkManager;

    public static GameState Instance { get; private set; }
    public bool IsInPlay { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        matchTracker = GetComponent<MatchTracker>();
        dodgeballNetworkManager = (DodgeballNetworkManager)NetworkManager.singleton;
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
        foreach (PlayerConnection playerConnection in dodgeballNetworkManager.PlayerConnections)
            if (playerConnection.IsLeftTeam)
                leftTeamPlayerCount += 1;
            else
                rightTeamPlayerCount += 1;

        return leftTeamPlayerCount > 0 && rightTeamPlayerCount > 0;
    }

    #endregion
}

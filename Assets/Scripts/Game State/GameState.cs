using UnityEngine;
using Mirror;
using System.Collections;

// Properties: static Instance, IsInPlay 
// Methods: [Server] StartGame, [Server] EndGame, static IsValidTeamComposition
public class GameState : NetworkBehaviour
{
    private Room room;
    private MatchTracker matchTracker;
    [SerializeField] private float timeToWaitForAllPlayersToConnect = 5f;

    public static GameState Instance { get; private set; }
    public bool IsInPlay { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        matchTracker = GetComponent<MatchTracker>();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [ServerCallback]
    private void Start()
    {
        room = (Room)NetworkManager.singleton;
        StartCoroutine(StartGame());
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
    private IEnumerator StartGame()
    {
        yield return WaitForAllPlayersToConnect();
        // Temporarily disabled for easier debugging
        //if (IsValidTeamComposition()gg)
        matchTracker.StartMatch();
    }

    [Server]
    private IEnumerator WaitForAllPlayersToConnect()
    {
        int secondsPassed = 0;
        while (secondsPassed < timeToWaitForAllPlayersToConnect)
        {
            if (room.Players.Count == room.Connections.Count)
                break;
            secondsPassed += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    [Server]
    public static bool IsValidTeamComposition()
    {
        return CountConnectionsOfTeam(Team.Left) > 0 &&
            CountConnectionsOfTeam(Team.Right) > 0;
    }

    [Server]
    private static int CountConnectionsOfTeam(Team team)
    {
        Room room = (Room)NetworkManager.singleton;
        int connectionCount = 0;
        foreach (Connection connection in room.Connections)
        {
            var playerData = connection.PlayerData;
            if (playerData.Team == team)
                connectionCount += 1;
        }
        return connectionCount;
    }

    [Server]
    private void EndGame(bool isLeftTeamWin)
    {
        matchTracker.EndMatch(isLeftTeamWin);
    }

    [ServerCallback]
    private void SubscribeEvents()
    {
        MatchTracker.ServerMatchStarted += HandleMatchStarted;
        PlayerTracker.ServerATeamLeft += HandleATeamLeft;
        MatchTracker.ServerMatchEnded += HandleMatchEnded;
    }

    [Server]
    private void HandleMatchStarted()
    {
        IsInPlay = true;
    }

    [Server]
    private void HandleATeamLeft(bool isLeftTeam)
    {
        EndGame(!isLeftTeam);
    }

    [Server]
    private void HandleMatchEnded()
    {
        IsInPlay = false;
    }

    [ServerCallback]
    private void UnsubscribeEvents()
    {
        MatchTracker.ServerMatchStarted -= HandleMatchStarted;
        PlayerTracker.ServerATeamLeft -= HandleATeamLeft;
        MatchTracker.ServerMatchEnded -= HandleMatchEnded;
    }

    #endregion
}

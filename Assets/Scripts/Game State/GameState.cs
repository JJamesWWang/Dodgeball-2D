using UnityEngine;
using Mirror;
using System.Collections;

// Methods: [Server] StartGame, [Server] EndGame, static IsValidTeamComposition
public class GameState : NetworkBehaviour
{
    private Room room;
    private MatchTracker matchTracker;
    [SerializeField] private float timeToWaitForAllPlayersToConnect = 5f;

    public bool IsInPlay { get; private set; }

    [ServerCallback]
    private void Awake()
    {
        matchTracker = GetComponent<MatchTracker>();
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
        StopAllCoroutines();
    }

    #region Server

    [Server]
    private IEnumerator StartGame()
    {
        yield return WaitForAllPlayersToConnect();
        // Temporarily disabled for easier debugging
        //if (IsValidTeamComposition())
        matchTracker.StartMatch();
    }

    [Server]
    private IEnumerator WaitForAllPlayersToConnect()
    {
        int secondsPassed = 0;
        while (secondsPassed < timeToWaitForAllPlayersToConnect)
        {
            yield return new WaitForSeconds(1f);
            secondsPassed += 1;
            if (AreAllPlayersReady())
                break;
        }
    }

    [Server]
    private bool AreAllPlayersReady()
    {
        foreach (Player player in room.Players)
            if (!player.Ready)
                return false;
        return true;
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
        if (IsInPlay)
            EndGame(!isLeftTeam);
    }

    [Server]
    private void HandleMatchEnded(bool _isLeftTeamWin)
    {
        IsInPlay = false;
    }

    [ServerCallback]
    private void UnsubscribeEvents()
    {
        MatchTracker.ServerMatchStarted += HandleMatchStarted;
        PlayerTracker.ServerATeamLeft -= HandleATeamLeft;
        MatchTracker.ServerMatchEnded += HandleMatchEnded;
    }

    #endregion
}

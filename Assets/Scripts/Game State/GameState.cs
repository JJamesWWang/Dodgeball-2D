using UnityEngine;
using Mirror;
using System.Collections;

// Properties: static Instance, IsInPlay 
// Methods: [Server] StartGame, [Server] EndGame
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

    [Server]
    private void EndGame(bool isLeftTeamWin)
    {
        matchTracker.EndMatch(isLeftTeamWin);
    }

    [Server]
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

    [Server]
    private void UnsubscribeEvents()
    {
        MatchTracker.ServerMatchStarted -= HandleMatchStarted;
        PlayerTracker.ServerATeamLeft -= HandleATeamLeft;
        MatchTracker.ServerMatchEnded -= HandleMatchEnded;
    }

    #endregion
}

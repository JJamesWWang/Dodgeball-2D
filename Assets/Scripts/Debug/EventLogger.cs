using UnityEngine;
using Mirror;
using Steamworks;
using Steamworks.Data;

// Also keeps track of all events in the code.
public class EventLogger : MonoBehaviour
{
    [SerializeField] private bool isLoggingOn;
    private static EventLogger singleton;
    private void Awake()
    {
        if (singleton != null && singleton != this)
            Destroy(singleton.gameObject);
        else
            singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    // Subscribing in order of appearance in Scripts folder tree
    private void SubscribeEvents()
    {
        if (!isLoggingOn) { return; }
        SubscribeGameStateEvents();
        SubscribeNetworkingEvents();
        SubscribeSteamEvents();
        SubscribePlayerEvents();
        SubscribeUIEvents();
    }

    private void SubscribeGameStateEvents()
    {
        Dodgeball.ServerDodgeballSpawned += HandleServerDodgeballSpawned;
        Dodgeball.ServerDodgeballDespawned += HandleServerDodgeballDespawned;
        MatchTracker.ServerMatchStarted += HandleServerMatchStarted;
        MatchTracker.ServerMatchEnded += HandleServerMatchEnded;
        MatchTracker.ClientMatchStarted += HandleClientMatchStarted;
        MatchTracker.ClientMatchEnded += HandleClientMatchEnded;
        PlayerTracker.ServerPlayerEliminated += HandleServerPlayerEliminated;
        PlayerTracker.ServerATeamLeft += HandleServerATeamLeft;
        RoundTracker.ServerRoundEnded += HandleServerRoundEnded;
        RoundTracker.ClientCountdownStarted += HandleClientCountdownStarted;
        RoundTracker.ClientRoundEnded += HandleClientRoundEnded;
        ScoreTracker.ServerScoreUpdated += HandleScoreUpdated;
        ScoreTracker.ClientScoreUpdated += HandleClientScoreUpdated;
    }

    // Events deemed unimportant are not logged and left as skeletons
    private void HandleServerDodgeballSpawned(Dodgeball dodgeball)
    {
    }

    private void HandleServerDodgeballDespawned(Dodgeball dodgeball)
    {
    }

    private void HandleServerMatchStarted()
    {
        Debug.Log("SERVER: STATE: Match started.");
    }

    private void HandleServerMatchEnded(bool isLeftTeamWin)
    {
        string winningTeam = isLeftTeamWin ? "left" : "right";
        Debug.Log($"SERVER: STATE: Match ended. The winner was the {winningTeam} team.");
    }

    private void HandleClientMatchStarted()
    {
        if (NetworkServer.active) { return; }
        Debug.Log("CLIENT: STATE: Match started.");
    }

    private void HandleClientMatchEnded(bool isLeftTeamWin)
    {
        if (NetworkServer.active) { return; }
        Debug.Log($"CLIENT: STATE: Match ended");
    }

    private void HandleServerPlayerEliminated(Player player)
    {
        Debug.Log($"SERVER: PLAYER: Player with username {player.Username} was eliminated.");
    }

    private void HandleServerATeamLeft(bool isLeftTeam)
    {
        string teamThatLeft = isLeftTeam ? "left" : "right";
        Debug.Log($"SERVER: STATE: The {teamThatLeft} team has no remaining players.");
    }

    private void HandleServerRoundEnded(bool isLeftTeamWin)
    {
        string teamThatWon = isLeftTeamWin ? "left" : "right";
        Debug.Log($"SERVER: STATE: Round ended. The winner was the {teamThatWon} team.");
    }

    private void HandleClientCountdownStarted(float timeToStartRound)
    {
    }

    private void HandleClientRoundEnded(bool isLeftTeamWin)
    {
        if (NetworkServer.active) { return; }
        Debug.Log("CLIENT: STATE: Round ended.");
    }

    private void HandleScoreUpdated(int leftTeamScore, int rightTeamScore)
    {
    }

    private void HandleClientScoreUpdated(int leftTeamScore, int rightTeamScore)
    {
    }

    private void SubscribeNetworkingEvents()
    {
        Connection.ClientConnectionStarted += HandleClientConnectionStarted;
        Connection.ClientLocalConnectionStarted += HandleClientLocalConnectionStarted;
        Connection.ClientConnectionStopped += HandleClientConnectionStopped;
        Room.ClientConnected += HandleClientConnected;
        Room.ClientDisconnected += HandleClientDisconnected;
    }

    private void HandleClientConnectionStarted(Connection connection)
    {
        Debug.Log("CLIENT: Client started.");
    }

    private void HandleClientLocalConnectionStarted(Connection connection)
    {
        Debug.Log("CLIENT: Client local started.");
    }

    private void HandleClientConnectionStopped(Connection connection)
    {
        Debug.Log("CLIENT: Client stopped.");
    }

    private void HandleClientConnected()
    {
        Debug.Log("CLIENT: Client connected.");
    }

    private void HandleClientDisconnected()
    {
        Debug.Log("CLIENT: Client disconnected.");
    }

    private void SubscribeSteamEvents()
    {
        SteamRoom.ClientCreateLobbyAttempted += HandleClientCreateLobbyAttempted;
        SteamRoom.ClientCreateLobbyFailed += HandleClientCreateLobbyFailed;
        SteamMatchmaking.OnLobbyCreated += HandleLobbyCreated;
        SteamRoom.ClientGameLobbyJoinRequested += HandleClientGameLobbyJoinRequested;
        SteamFriends.OnGameLobbyJoinRequested += HandleGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyEntered += HandleLobbyEntered;
    }

    private void HandleClientCreateLobbyAttempted()
    {
        Debug.Log("HOST: STEAM: Attempted to create lobby.");
    }

    private void HandleClientCreateLobbyFailed()
    {
        Debug.Log("HOST: STEAM: Attempt to create lobby failed.");
    }

    private void HandleLobbyCreated(Result result, Lobby lobby)
    {
        Debug.Log($"HOST: STEAM: Lobby creation attempt ended in result {result}.");
    }

    private void HandleClientGameLobbyJoinRequested()
    {
        Debug.Log("CLIENT: STEAM: Attempted to join lobby.");
    }

    private void HandleGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Debug.Log("CLIENT: STEAM: Requested to join lobby.");
    }

    private void HandleLobbyEntered(Lobby lobby)
    {
        Debug.Log("CLIENT: STEAM: Joined lobby.");
    }

    private void SubscribePlayerEvents()
    {
        Player.ServerPlayerHit += HandleServerPlayerHit;
        Player.ServerPlayerDisconnected += HandleServerPlayerDisconnected;
        Player.ClientLocalPlayerStarted += HandleClientLocalPlayerStarted;
        Player.ClientPlayerSpawned += HandleClientPlayerSpawned;
        PlayerData.ClientPlayerDataUpdated += HandleClientPlayerDataUpdated;
    }

    private void HandleServerPlayerHit(Player player)
    {
    }

    private void HandleServerPlayerDisconnected(Player player)
    {
        Debug.Log($"SERVER: PLAYER: Player with username {player.Username} disconnected.");
    }

    private void HandleClientLocalPlayerStarted(Player player)
    {
        Debug.Log($"CLIENT: PLAYER: Local Player with username {player.Username} started.");
    }

    private void HandleClientPlayerSpawned(Player player)
    {
        Debug.Log($"CLIENT: PLAYER: Player with username {player.Username} spawned.");
    }

    private void HandleClientPlayerDataUpdated(uint connectionNetId, string propertyName, object propertyValue)
    {
        Debug.Log($"CLIENT: PLAYER: Player updated property with name {propertyName} to value {propertyValue}");
    }

    private void SubscribeUIEvents()
    {
        GameOverUI.ClientGameOverUIToggled -= HandleClientGameOverUIToggled;
    }

    private void HandleClientGameOverUIToggled(bool isToggledOn)
    {
    }


    private void UnsubscribeEvents()
    {
        if (!isLoggingOn) { return; }
        UnsubscribeGameStateEvents();
        UnsubscribeNetworkingEvents();
        UnsubscribeSteamEvents();
        UnsubscribePlayerEvents();
        UnsubscribeUIEvents();
    }

    private void UnsubscribeGameStateEvents()
    {
        Dodgeball.ServerDodgeballSpawned -= HandleServerDodgeballSpawned;
        Dodgeball.ServerDodgeballDespawned -= HandleServerDodgeballDespawned;
        MatchTracker.ServerMatchStarted -= HandleServerMatchStarted;
        MatchTracker.ServerMatchEnded -= HandleServerMatchEnded;
        MatchTracker.ClientMatchStarted -= HandleClientMatchStarted;
        MatchTracker.ClientMatchEnded -= HandleClientMatchEnded;
        PlayerTracker.ServerPlayerEliminated -= HandleServerPlayerEliminated;
        PlayerTracker.ServerATeamLeft -= HandleServerATeamLeft;
        RoundTracker.ServerRoundEnded -= HandleServerRoundEnded;
        RoundTracker.ClientCountdownStarted -= HandleClientCountdownStarted;
        RoundTracker.ClientRoundEnded -= HandleClientRoundEnded;
        ScoreTracker.ServerScoreUpdated -= HandleScoreUpdated;
        ScoreTracker.ClientScoreUpdated -= HandleClientScoreUpdated;
    }

    private void UnsubscribeNetworkingEvents()
    {
        Connection.ClientConnectionStarted -= HandleClientConnectionStarted;
        Connection.ClientLocalConnectionStarted -= HandleClientLocalConnectionStarted;
        Connection.ClientConnectionStopped -= HandleClientConnectionStopped;
        Room.ClientConnected -= HandleClientConnected;
        Room.ClientDisconnected -= HandleClientDisconnected;
    }

    private void UnsubscribeSteamEvents()
    {
        SteamRoom.ClientCreateLobbyAttempted -= HandleClientCreateLobbyAttempted;
        SteamRoom.ClientCreateLobbyFailed -= HandleClientCreateLobbyFailed;
        SteamMatchmaking.OnLobbyCreated -= HandleLobbyCreated;
        SteamRoom.ClientGameLobbyJoinRequested -= HandleClientGameLobbyJoinRequested;
        SteamFriends.OnGameLobbyJoinRequested -= HandleGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyEntered -= HandleLobbyEntered;
    }


    private void UnsubscribePlayerEvents()
    {
        Player.ServerPlayerHit -= HandleServerPlayerHit;
        Player.ServerPlayerDisconnected -= HandleServerPlayerDisconnected;
        Player.ClientLocalPlayerStarted -= HandleClientLocalPlayerStarted;
        Player.ClientPlayerSpawned -= HandleClientPlayerSpawned;
        PlayerData.ClientPlayerDataUpdated -= HandleClientPlayerDataUpdated;
    }

    private void UnsubscribeUIEvents()
    {
        GameOverUI.ClientGameOverUIToggled -= HandleClientGameOverUIToggled;
    }
}

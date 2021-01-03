using UnityEngine;
using Mirror;
using System;
using Steamworks;
using Steamworks.Data;

// Properties: Lobby 
// Events: HostStarted, HostStopped, ClientGameLobbyJoinRequested, ClientLobbyEntered
public class SteamRoom : Room
{
    private Lobby lobby;

    public Lobby Lobby { get { return lobby; } }

    public static event Action ClientCreateLobbyAttempted;
    public static event Action ClientCreateLobbyFailed;
    public static event Action ClientGameLobbyJoinRequested;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    #region Server

    public void CreateSteamLobby()
    {
        SteamMatchmaking.CreateLobbyAsync(maxConnections);
        ClientCreateLobbyAttempted?.Invoke();
    }

    public override void OnRoomStopHost()
    {
        base.OnRoomStopHost();
        lobby.Leave();
    }

    #endregion Server

    private void SubscribeEvents()
    {
        SteamMatchmaking.OnLobbyCreated += HandleLobbyCreated;
        SteamFriends.OnGameLobbyJoinRequested += HandleGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyEntered += HandleLobbyEntered;
    }

    // Server
    private void HandleLobbyCreated(Result result, Lobby lobby)
    {
        Debug.Log($"Attempted to create Lobby with result {result}.");
        if (result != Result.OK)
        {
            Disconnect();
            ClientCreateLobbyFailed?.Invoke();
            return;
        }
        this.lobby = SetUpLobby(lobby);
        StartHost();
    }

    private Lobby SetUpLobby(Lobby lobby)
    // Server
    {
        lobby.SetFriendsOnly();
        var steamId = SteamClient.SteamId.ToString();
        lobby.SetData(nameof(networkAddress), steamId);
        return lobby;
    }

    // Client
    private void HandleGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Debug.Log("Game Lobby join requested.");
        if (NetworkServer.active || NetworkClient.isConnected) { return; }
        SteamMatchmaking.JoinLobbyAsync(lobby.Id);
        ClientGameLobbyJoinRequested?.Invoke();
    }

    // Client
    private void HandleLobbyEntered(Lobby lobby)
    {
        if (NetworkServer.active) { return; }
        Debug.Log("Lobby entered.");
        networkAddress = lobby.GetData(nameof(networkAddress));
        StartClient();
    }

    private void UnsubscribeEvents()
    {
        SteamMatchmaking.OnLobbyCreated -= HandleLobbyCreated;
        SteamFriends.OnGameLobbyJoinRequested -= HandleGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyEntered -= HandleLobbyEntered;
    }

}

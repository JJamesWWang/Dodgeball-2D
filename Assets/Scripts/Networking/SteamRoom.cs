using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Mirror;

public class SteamRoom : Room
{
    private Lobby lobby;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public override void OnRoomStartHost()
    {
        base.OnRoomStartHost();
        SteamMatchmaking.CreateLobbyAsync(maxConnections);
    }

    public override void OnRoomStopHost()
    {
        base.OnRoomStopHost();
        lobby.Leave();
    }

    private void SubscribeEvents()
    {
        SteamMatchmaking.OnLobbyCreated += HandleLobbyCreated;
        SteamFriends.OnGameLobbyJoinRequested += HandleGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyEntered += HandleLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += HandleLobbyMemberJoined;
    }

    // Server
    private void HandleLobbyCreated(Result result, Lobby lobby)
    {
        Debug.Log($"Created Lobby with result {result}.");
        if (result != Result.OK) { return; }
        this.lobby = SetUpLobby(lobby);
        Debug.Log("Lobby successfully created.");
        Debug.Log($"Player ID: {SteamClient.SteamId}");
        Debug.Log($"Lobby ID: {lobby.Id}");
    }

    // Server
    private Lobby SetUpLobby(Lobby lobby)
    {
        lobby.SetFriendsOnly();
        var steamId = SteamClient.SteamId.ToString();
        lobby.SetData(nameof(networkAddress), steamId);
        return lobby;
    }

    // Client
    private void HandleGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        if (NetworkServer.active)
            return;
        Debug.Log("Game Lobby join requested.");
        SteamMatchmaking.JoinLobbyAsync(lobby.Id);
    }

    // Client
    private void HandleLobbyEntered(Lobby lobby)
    {
        if (NetworkServer.active)
            return;
        Debug.Log("Lobby entered.");
        networkAddress = lobby.GetData(nameof(networkAddress));
        StartClient();
    }

    // Client - Temporarily see what this does
    private void HandleLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        if (NetworkServer.active)
            return;
        Debug.Log($"Lobby member joined with friend ID: {friend.Id}");
        SteamFriends.SetPlayedWith(friend.Id);
    }

    private void UnsubscribeEvents()
    {
        SteamMatchmaking.OnLobbyCreated -= HandleLobbyCreated;
        SteamFriends.OnGameLobbyJoinRequested -= HandleGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyEntered -= HandleLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= HandleLobbyMemberJoined;
    }
}

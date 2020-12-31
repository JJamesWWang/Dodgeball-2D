using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Steamworks;
using Steamworks.Data;

public class LobbyUI : NetworkBehaviour
{
    private Room room;
    private PlayerData localPlayerData;
    [SerializeField] private GameObject lobbyUIParent;
    [SerializeField] private TMP_Text leftTeamPlayersText;
    [SerializeField] private TMP_Text rightTeamPlayersText;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button startButton;

    #region General

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void HandleLeaveClick()
    {
        room.Disconnect();
    }

    #endregion

    #region Server

    [Server]
    public void HandleStartClick()
    {
        if (GameState.IsValidTeamComposition())
            room.ServerChangeScene(room.GameplayScene);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        room = (Room)NetworkManager.singleton;
        if (NetworkServer.active)
            startButton.gameObject.SetActive(true);
        CheckInit();
    }

    [Client]
    private void CheckInit()
    {
        var connection = Connection.LocalConnection;
        if (connection != null)
            Init(connection);
    }

    [Client]
    private void Init(Connection connection)
    {
        localPlayerData = connection.PlayerData;
        ConstructPlayersText();
        usernameInput.text = localPlayerData.Username;
    }

    [Client]
    private void ConstructPlayersText()
    {
        leftTeamPlayersText.text = "";
        rightTeamPlayersText.text = "";
        foreach (Connection connection in room.Connections)
            AddToPlayersText(connection);
    }

    [Client]
    private void AddToPlayersText(Connection connection)
    {
        var playerData = connection.PlayerData;
        if (playerData.IsLeftTeam)
            leftTeamPlayersText.text += $"{playerData.Username}\n";
        else if (playerData.IsRightTeam)
            rightTeamPlayersText.text += $"{playerData.Username}\n";
    }

    [Client]
    public void HandleJoinLeftTeamClick()
    {
        localPlayerData.CmdSetTeam(Team.Left);
    }

    [Client]
    public void HandleSpectateClick()
    {
        localPlayerData.CmdSetTeam(Team.Spectator);
    }

    [Client]
    public void HandleJoinRightTeamClick()
    {
        localPlayerData.CmdSetTeam(Team.Right);
    }

    [Client]
    public void HandleSaveClick()
    {
        string username = usernameInput.text;
        localPlayerData.CmdSetUsername(username);
    }

    [ClientCallback]
    private void SubscribeEvents()
    {
        Connection.ClientLocalConnected += HandleLocalPlayerConnected;
        Connection.ClientConnected += HandlePlayerConnected;
        Connection.ClientDisconnected += HandlePlayerDisconnected;
        PlayerData.ClientPlayerDataUpdated += HandlePlayerDataUpdated;
        SteamMatchmaking.OnLobbyMemberJoined += HandleLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberDisconnected += HandleLobbyMemberDisconnected;
    }

    [Client]
    private void HandleLocalPlayerConnected(Connection connection)
    {
        Init(connection);
    }

    [Client]
    private void HandlePlayerConnected(Connection connection)
    {
        ConstructPlayersText();
    }

    [Client]
    private void HandlePlayerDisconnected(Connection connection)
    {
        ConstructPlayersText();
    }

    [Client]
    private void HandlePlayerDataUpdated(uint _netId, string _propertyName, object _value)
    {
        if (room != null)
            ConstructPlayersText();
    }

    [Client]
    private void HandleLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        ConstructPlayersText();
    }

    [Client]
    private void HandleLobbyMemberDisconnected(Lobby lobby, Friend friend)
    {
        ConstructPlayersText();
    }

    [ClientCallback]
    private void UnsubscribeEvents()
    {
        Connection.ClientLocalConnected -= HandleLocalPlayerConnected;
        Connection.ClientConnected -= HandlePlayerConnected;
        Connection.ClientDisconnected -= HandlePlayerDisconnected;
        PlayerData.ClientPlayerDataUpdated -= HandlePlayerDataUpdated;
        SteamMatchmaking.OnLobbyMemberJoined -= HandleLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberDisconnected -= HandleLobbyMemberDisconnected;
    }

    #endregion

}

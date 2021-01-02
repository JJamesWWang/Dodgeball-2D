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
    [SerializeField] private TMP_Text leftTeamPlayersText;
    [SerializeField] private TMP_Text rightTeamPlayersText;
    [SerializeField] private Button inviteFriendButton;
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

    public void HandleLeaveClicked()
    {
        room.Disconnect();
    }

    #endregion

    #region Server

    [Server]
    public void HandleStartClicked()
    {
        // Temporarily disabling for easier testing
        //if (GameState.IsValidTeamComposition())
        room.ServerChangeScene(room.GameplayScene);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        room = (Room)NetworkManager.singleton;
        if (NetworkServer.active)
            startButton.gameObject.SetActive(true);
        if (NetworkManager.singleton is SteamRoom)
            inviteFriendButton.gameObject.SetActive(true);
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
        if (room == null) { return; }
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
    public void HandleJoinLeftTeamClicked()
    {
        localPlayerData.CmdSetTeam(Team.Left);
    }

    [Client]
    public void HandleSpectateClicked()
    {
        localPlayerData.CmdSetTeam(Team.Spectator);
    }

    [Client]
    public void HandleInviteFriendClicked()
    {
        SteamRoom steamRoom = (SteamRoom)room;
        SteamFriends.OpenGameInviteOverlay(steamRoom.Lobby.Id);
    }

    [Client]
    public void HandleJoinRightTeamClicked()
    {
        localPlayerData.CmdSetTeam(Team.Right);
    }

    [Client]
    public void HandleSaveClicked()
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

    [ClientCallback]
    private void HandlePlayerDisconnected(Connection connection)
    {
        ConstructPlayersText();
    }

    [Client]
    private void HandlePlayerDataUpdated(uint _netId, string _propertyName, object _value)
    {
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

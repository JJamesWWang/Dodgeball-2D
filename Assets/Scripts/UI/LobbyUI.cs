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
    private CommandLogger commandLogger;

    #region General

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Awake()
    {
        commandLogger = CommandLogger.singleton;
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
        Debug.Log("SERVER: Player clicked lobby start button.");
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
        if (room is SteamRoom)
        {
            usernameInput.text = SteamClient.Name;
            localPlayerData.CmdSetUsername(SteamClient.Name);
            commandLogger.LogCommand($"Player wants to set username to {SteamClient.Name}.");
        }
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
        commandLogger.LogCommand($"Player wants to set team to left.");
    }

    [Client]
    public void HandleSpectateClicked()
    {
        localPlayerData.CmdSetTeam(Team.Spectator);
        commandLogger.LogCommand($"Player wants to set team to spectator.");
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
        commandLogger.LogCommand($"Player wants to set team to right.");
    }

    [Client]
    public void HandleSaveClicked()
    {
        string username = usernameInput.text;
        localPlayerData.CmdSetUsername(username);
        commandLogger.LogCommand($"Player wants to set username to {username}");
    }

    [ClientCallback]
    private void SubscribeEvents()
    {
        Connection.ClientLocalConnectionStarted += HandleClientLocalConnectionStarted;
        Connection.ClientConnectionStarted += HandleClientConnectionStarted;
        Connection.ClientConnectionStopped += HandleClientConnectionStopped;
        PlayerData.ClientPlayerDataUpdated += HandlePlayerDataUpdated;
        SteamMatchmaking.OnLobbyMemberJoined += HandleLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberDisconnected += HandleLobbyMemberDisconnected;
    }

    [Client]
    private void HandleClientLocalConnectionStarted(Connection connection)
    {
        Init(connection);
    }

    [Client]
    private void HandleClientConnectionStarted(Connection connection)
    {
        ConstructPlayersText();
    }

    [ClientCallback]
    private void HandleClientConnectionStopped(Connection connection)
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
        Connection.ClientLocalConnectionStarted -= HandleClientLocalConnectionStarted;
        Connection.ClientConnectionStarted -= HandleClientConnectionStarted;
        Connection.ClientConnectionStopped -= HandleClientConnectionStopped;
        PlayerData.ClientPlayerDataUpdated -= HandlePlayerDataUpdated;
        SteamMatchmaking.OnLobbyMemberJoined -= HandleLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberDisconnected -= HandleLobbyMemberDisconnected;
    }

    #endregion

}

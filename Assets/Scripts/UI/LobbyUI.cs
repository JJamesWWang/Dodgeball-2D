using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUIParent;
    [SerializeField] private TMP_Text leftTeamPlayersText;
    [SerializeField] private TMP_Text rightTeamPlayersText;
    [SerializeField] private Button joinLeftTeamButton;
    [SerializeField] private Button joinRightTeamButton;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button startButton;
    private Room room;
    private PlayerData localPlayerData;

    private void Awake()
    {
        MatchTracker.ClientMatchStarted += HandleMatchStarted;
        MatchTracker.ClientMatchEnded += HandleMatchEnded;
        Connection.ClientLocalConnected += HandleLocalPlayerConnected;
        Connection.ClientConnected += HandlePlayerConnected;
        Connection.ClientDisconnected += HandlePlayerDisconnected;
        PlayerData.ClientPlayerInfoUpdated += HandlePlayerInfoUpdated;
    }

    private void Start()
    {
        room = (Room)NetworkManager.singleton;
        if (NetworkServer.active)
            startButton.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        MatchTracker.ClientMatchStarted -= HandleMatchStarted;
        MatchTracker.ClientMatchEnded -= HandleMatchEnded;
        Connection.ClientLocalConnected -= HandleLocalPlayerConnected;
        Connection.ClientConnected -= HandlePlayerConnected;
        Connection.ClientDisconnected -= HandlePlayerDisconnected;
        PlayerData.ClientPlayerInfoUpdated -= HandlePlayerInfoUpdated;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            lobbyUIParent.gameObject.SetActive(!lobbyUIParent.gameObject.activeSelf);
        }
    }

    private void HandleMatchStarted()
    {
        lobbyUIParent.gameObject.SetActive(false);
        joinLeftTeamButton.gameObject.SetActive(false);
        joinRightTeamButton.gameObject.SetActive(false);
    }

    private void HandleMatchEnded(bool _isLeftTeamWin)
    {
        lobbyUIParent.gameObject.SetActive(true);
        joinLeftTeamButton.gameObject.SetActive(true);
        joinRightTeamButton.gameObject.SetActive(true);
    }

    private void HandleLocalPlayerConnected(Connection connection)
    {
        localPlayerData = connection.GetComponent<PlayerData>();
        InitLobbyUI();
    }

    private void HandlePlayerConnected(Connection connection)
    {
        ConstructPlayersText();
    }

    private void HandlePlayerDisconnected(Connection connection)
    {
        ConstructPlayersText();
    }

    private void InitLobbyUI()
    {
        ConstructPlayersText();
        usernameInput.text = localPlayerData.Username;
    }

    private void ConstructPlayersText()
    {
        leftTeamPlayersText.text = "";
        rightTeamPlayersText.text = "";
        foreach (Connection connection in room.Connections)
            AddToPlayersText(connection);
    }

    private void AddToPlayersText(Connection connection)
    {
        var playerData = connection.GetComponent<PlayerData>();
        if (playerData.IsLeftTeam)
            leftTeamPlayersText.text += $"{playerData.Username}\n";
        else
            rightTeamPlayersText.text += $"{playerData.Username}\n";
    }

    private void HandlePlayerInfoUpdated(uint _netId, string _propertyName, object _value)
    {
        ConstructPlayersText();
    }

    public void HandleJoinLeftTeamClick()
    {
        localPlayerData.CmdSetIsLeftTeam(true);
    }

    public void HandleJoinRightTeamClick()
    {
        localPlayerData.CmdSetIsLeftTeam(false);
    }

    public void HandleSaveClick()
    {
        string username = usernameInput.text;
        localPlayerData.CmdSetUsername(username);
    }

    public void HandleLeaveClick()
    {
        if (NetworkServer.active)
            if (NetworkClient.isConnected)
                room.StopHost();
            else
                room.StopServer();
        else
            room.StopClient();
        SceneManager.LoadScene(0);
    }

    public void HandleStartClick()
    {
        room.ServerChangeScene(room.GameplayScene);
    }

}

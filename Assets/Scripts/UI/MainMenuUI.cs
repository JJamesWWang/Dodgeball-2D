using UnityEngine;
using Mirror;
using TMPro;
using System.Text.RegularExpressions;
using Steamworks;

public class MainMenuUI : MonoBehaviour
{
    private string defaultIPAddressText;
    private string connectingIPAddressText = "Connecting...";
    private string invalidIPAddressText = "Invalid IP Address.";
    private string failedToConnectText = "Couldn't connect.";

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    [SerializeField] private bool useSteam = false;
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private GameObject enterIPAddressParent;
    [SerializeField] private TMP_Text enterIPAddressText;
    [SerializeField] private TMP_InputField enterIPAddressInput;
    private DodgeballNetworkManager dodgeballNetworkManager;

    private void Start()
    {
        defaultIPAddressText = enterIPAddressText.text;
        dodgeballNetworkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        gameObject.SetActive(true);
        DodgeballNetworkManager.ClientDisconnected += HandleClientDisconnected;
        DodgeballNetworkManager.ClientConnected += HandleClientConnected;

        if (useSteam)
            InitSteam();

    }

    private void OnDestroy()
    {
        DodgeballNetworkManager.ClientDisconnected -= HandleClientDisconnected;
        DodgeballNetworkManager.ClientConnected -= HandleClientConnected;
    }

    public void HandleServerClick()
    {
        dodgeballNetworkManager.StartServer();
        // Temporarily hardcoding this
        dodgeballNetworkManager.ServerChangeScene("Map 1");
    }

    public void HandleHostClick()
    {
        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, dodgeballNetworkManager.maxConnections);
            return;
        }
        dodgeballNetworkManager.StartHost();
        // Temporarily hardcoding this
        dodgeballNetworkManager.ServerChangeScene("Map 1");
    }

    public void HandleClientClick()
    {
        enterIPAddressParent.SetActive(true);
        buttonsParent.SetActive(false);
    }

    public void HandleEnterIPAddressConnect()
    {
        string enteredIPAddress = enterIPAddressInput.text;
        if (string.IsNullOrEmpty(enteredIPAddress)) { return; }
        Regex validIPAddressChecker = new Regex("^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\\.(?!$)|$)){4}$");
        if (enteredIPAddress != "localhost" &&
            !validIPAddressChecker.IsMatch(enteredIPAddress)) 
        {
            enterIPAddressText.text = invalidIPAddressText;
            return; 
        }
        enterIPAddressText.text = connectingIPAddressText;
        dodgeballNetworkManager.networkAddress = enteredIPAddress;
        dodgeballNetworkManager.StartClient();
    }

    public void HandleEnterIPAddressClose()
    {
        enterIPAddressText.text = defaultIPAddressText;
        enterIPAddressParent.SetActive(false);
        buttonsParent.SetActive(true);
    }

    private void HandleClientConnected(NetworkConnection _conn)
    {
        // Temporarily hardcoding this
        dodgeballNetworkManager.ServerChangeScene("Map 1");
    }

    private void HandleClientDisconnected(NetworkConnection _conn)
    {
        enterIPAddressText.text = failedToConnectText;
    }

    private void InitSteam()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(HandleLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(HandleGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(HandleLobbyEntered);
    }

    private void HandleLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;
        dodgeballNetworkManager.StartHost();
        // Temporarily hardcoding this
        dodgeballNetworkManager.ServerChangeScene("Map 1");
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString());

    }

    private void HandleGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void HandleLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();
    }
}

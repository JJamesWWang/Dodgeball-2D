using UnityEngine;
using Mirror;
using TMPro;
using System.Text.RegularExpressions;

public class MainMenuUI : MonoBehaviour
{
    private string defaultIPAddressText;
    private string connectingIPAddressText = "Connecting...";
    private string invalidIPAddressText = "Invalid IP Address.";
    private string failedToConnectText = "Couldn't connect.";

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

}

using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;
using TMPro;

public class LocalhostUI : MonoBehaviour
{
    private Room room;

    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private GameObject enterIPAddressParent;
    [SerializeField] private TMP_Text enterIPAddressText;
    [SerializeField] private TMP_InputField enterIPAddressInput;

    [Header("Status Messages")]
    [SerializeField] private string defaultIPAddressMessage = "Enter IP Address:";
    [SerializeField] private string connectingIPAddressMessage = "Connecting...";
    [SerializeField] private string invalidIPAddressMessage = "Invalid IP Address.";
    [SerializeField] private string connectedToIPAddressMessage = "Connected!";
    [SerializeField] private string failedToConnectToIPAddressMessage = "Failed to connect.";


    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Start()
    {
        room = (Room)NetworkManager.singleton;
        enterIPAddressText.text = defaultIPAddressMessage;
    }

    public void HandleServerClicked()
    {
        room.StartServer();
    }

    public void HandleHostClicked()
    {
        room.StartHost();
    }

    public void HandleClientClicked()
    {
        enterIPAddressParent.SetActive(true);
        buttonsParent.SetActive(false);
    }

    public void HandleEnterIPAddressConnectClicked()
    {
        string enteredIPAddress = enterIPAddressInput.text;
        bool isValidIPAddress = IsValidIPAddress(enteredIPAddress);
        UpdateIPAddressText(isValidIPAddress);
        if (!isValidIPAddress) { return; }
        ConnectToIPAddress(enteredIPAddress);
    }

    private bool IsValidIPAddress(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress)) { return false; }
        Regex validIPAddressChecker = new Regex("^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\\.(?!$)|$)){4}$");
        return ipAddress == "localhost" ||
                validIPAddressChecker.IsMatch(ipAddress);
    }

    private void UpdateIPAddressText(bool isValidIPAddress)
    {
        if (isValidIPAddress)
            enterIPAddressText.text = connectingIPAddressMessage;
        else
            enterIPAddressText.text = invalidIPAddressMessage;
    }

    private void ConnectToIPAddress(string ipAddress)
    {
        room.networkAddress = ipAddress;
        room.StartClient();
    }

    public void HandleEnterIPAddressCloseClicked()
    {
        enterIPAddressText.text = defaultIPAddressMessage;
        enterIPAddressParent.SetActive(false);
        buttonsParent.SetActive(true);
    }

    private void SubscribeEvents()
    {
        Room.ClientConnected += HandleClientConnected;
        Room.ClientDisconnected += HandleClientDisconnected;
    }

    private void HandleClientConnected()
    {
        enterIPAddressText.text = connectedToIPAddressMessage;
    }

    private void HandleClientDisconnected()
    {
        enterIPAddressText.text = failedToConnectToIPAddressMessage;
    }

    private void UnsubscribeEvents()
    {
        Room.ClientConnected -= HandleClientConnected;
        Room.ClientDisconnected -= HandleClientDisconnected;
    }
}

using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    private string defaultIPAddressText;
    private string connectingIPAddressText = "Connecting...";
    private string invalidIPAddressText = "Invalid IP Address.";
    private Room room;

    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private GameObject enterIPAddressParent;
    [SerializeField] private TMP_Text enterIPAddressText;
    [SerializeField] private TMP_InputField enterIPAddressInput;

    private void Start()
    {
        defaultIPAddressText = enterIPAddressText.text;
        room = (Room)NetworkManager.singleton;
    }

    public void HandleServerClick()
    {
        room.StartServer();
    }

    public void HandleHostClick()
    {
        room.StartHost();
    }

    public void HandleClientClick()
    {
        enterIPAddressParent.SetActive(true);
        buttonsParent.SetActive(false);
    }

    public void HandleEnterIPAddressConnect()
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
            enterIPAddressText.text = connectingIPAddressText;
        else
            enterIPAddressText.text = invalidIPAddressText;
    }

    private void ConnectToIPAddress(string ipAddress)
    {
        room.networkAddress = ipAddress;
        room.StartClient();
    }

    public void HandleEnterIPAddressClose()
    {
        enterIPAddressText.text = defaultIPAddressText;
        enterIPAddressParent.SetActive(false);
        buttonsParent.SetActive(true);
    }
}

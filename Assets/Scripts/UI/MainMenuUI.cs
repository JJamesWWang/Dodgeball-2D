using UnityEngine;
using Mirror;
using TMPro;
using System.Text.RegularExpressions;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private GameObject enterIPAddressParent;
    [SerializeField] private TMP_InputField enterIPAddressInput;
    private DodgeballNetworkManager dodgeballNetworkManager;

    private void Start()
    {
        dodgeballNetworkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        gameObject.SetActive(true);
    }

    public void HandleServerClick()
    {
        dodgeballNetworkManager.StartServer();
    }

    public void HandleHostClick()
    {
        dodgeballNetworkManager.StartHost();
    }

    public void HandleClientClick()
    {
        enterIPAddressParent.SetActive(true);
        buttonsParent.SetActive(false);
    }

    public void HandleEnterIPAddressConnect()
    {
        string enteredIPAddress = enterIPAddressInput.text;
        if (enteredIPAddress == "") { return; }
        Regex validIPAddressChecker = new Regex("^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\\.(?!$)|$)){4}$");
        if (enteredIPAddress != "localhost" &&
            !validIPAddressChecker.IsMatch(enteredIPAddress)) { return; }
        dodgeballNetworkManager.networkAddress = enteredIPAddress;
        dodgeballNetworkManager.StartClient();
        HandleEnterIPAddressClose();
    }

    public void HandleEnterIPAddressClose()
    {
        enterIPAddressParent.SetActive(false);
        buttonsParent.SetActive(true);
    }

}

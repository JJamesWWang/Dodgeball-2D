using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;
using System;

public class SteamUI : MonoBehaviour
{
    private SteamRoom room;
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button closeStatusPanelButton;

    [Header("Status Messages")]
    [SerializeField] private string creatingLobbyMessage = "Creating Lobby...";
    [SerializeField] private string failedToCreateLobbyMessage = "Couldn't create Lobby.";
    [SerializeField] private string joiningLobbyMessage = "Joining Lobby...";
    [SerializeField] private string failedToJoinLobbyMessage = "Couldn't join Lobby.";


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
        room = (SteamRoom)NetworkManager.singleton;
    }

    public void HandleHostLobbyClicked()
    {
        room.CreateSteamLobby();
    }

    public void HandleJoinFriendClicked()
    {
        SteamFriends.OpenOverlay("friends");
    }

    public void HandleCloseStatusPanelButtonClicked()
    {
        HideStatusPanel();
    }

    private void HideStatusPanel()
    {
        closeStatusPanelButton.gameObject.SetActive(false);
        statusPanel.SetActive(false);
        buttonsPanel.SetActive(true);
    }

    private void SubscribeEvents()
    {
        SteamRoom.ClientCreateLobbyAttempted += HandleCreateLobbyAttempted;
        SteamRoom.ClientCreateLobbyFailed += HandleCreateLobbyFailed;
        SteamRoom.ClientGameLobbyJoinRequested += HandleClientGameLobbyJoinRequested;
        Room.ClientConnected += HandleClientConnected;
        Room.ClientDisconnected += HandleClientDisconnected;
    }

    private void HandleCreateLobbyAttempted()
    {
        ShowStatusPanel();
        statusText.text = creatingLobbyMessage;
    }

    private void ShowStatusPanel()
    {
        buttonsPanel.SetActive(false);
        statusPanel.SetActive(true);
    }

    private void HandleCreateLobbyFailed()
    {
        ShowStatusPanel();
        statusText.text = failedToCreateLobbyMessage;
        closeStatusPanelButton.gameObject.SetActive(true);
    }

    private void HandleClientGameLobbyJoinRequested()
    {
        ShowStatusPanel();
        statusText.text = joiningLobbyMessage;
    }

    // Temporarily here, not sure if it's actually necessary.
    private void HandleClientConnected()
    {
        ShowStatusPanel();
        statusText.text = joiningLobbyMessage;
    }

    private void HandleClientDisconnected()
    {
        ShowStatusPanel();
        statusText.text = failedToJoinLobbyMessage;
        closeStatusPanelButton.gameObject.SetActive(true);
    }

    private void UnsubscribeEvents()
    {
        SteamRoom.ClientCreateLobbyAttempted -= HandleCreateLobbyAttempted;
        SteamRoom.ClientCreateLobbyFailed -= HandleCreateLobbyFailed;
        SteamRoom.ClientGameLobbyJoinRequested -= HandleClientGameLobbyJoinRequested;
        Room.ClientConnected -= HandleClientConnected;
        Room.ClientDisconnected -= HandleClientDisconnected;
    }
}

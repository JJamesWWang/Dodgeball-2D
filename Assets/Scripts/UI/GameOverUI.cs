using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;
using System;
using System.Collections;
using TMPro;

// Events: ClientToggled
public class GameOverUI : MonoBehaviour
{
    private Room room;
    [SerializeField] private TMP_Text leftTeamWinsText;
    [SerializeField] private TMP_Text rightTeamWinsText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text waitingForHostText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button disconnectButton;
    [SerializeField] private float timeToDelayShowDisconnectButton = 3f;

    /// <summary> bool: isToggledOn </summary>
    public static event Action<bool> ClientGameOverUIToggled;

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
        if (NetworkServer.active)
            restartButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ToggleGameOverUI();
    }

    private void ToggleGameOverUI()
    {
        gameOverPanel.SetActive(!gameOverPanel.activeSelf);
        ClientGameOverUIToggled?.Invoke(gameOverPanel.activeSelf);
    }

    public void HandleRestartClicked()
    {
        EventLogger.LogEvent("SERVER: Player clicked restart button.");
        room.ServerChangeScene(room.RoomScene);
    }

    public void HandleDisconnectClicked()
    {
        if (NetworkClient.active)
            EventLogger.LogEvent("CLIENT: Player clicked on disconnect button.");
        room.Disconnect();
    }

    private void SubscribeEvents()
    {
        MatchTracker.ClientMatchEnded += HandleMatchEnded;
    }

    private void HandleMatchEnded(bool isLeftTeamWin)
    {
        ShowGameOverPanel(isLeftTeamWin);
    }

    private void ShowGameOverPanel(bool isLeftTeamWin)
    {
        ShowWinnerText(isLeftTeamWin);
        if (!NetworkServer.active)
            waitingForHostText.gameObject.SetActive(true);
        gameOverPanel.SetActive(true);
        StartCoroutine(DelayShowDisconnectButton());
    }

    private IEnumerator DelayShowDisconnectButton()
    {
        disconnectButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeToDelayShowDisconnectButton);
        disconnectButton.gameObject.SetActive(true);
    }

    private void ShowWinnerText(bool isLeftTeamWin)
    {
        if (isLeftTeamWin)
            leftTeamWinsText.gameObject.SetActive(true);
        else
            rightTeamWinsText.gameObject.SetActive(true);
    }

    private void UnsubscribeEvents()
    {
        MatchTracker.ClientMatchEnded -= HandleMatchEnded;
    }

}

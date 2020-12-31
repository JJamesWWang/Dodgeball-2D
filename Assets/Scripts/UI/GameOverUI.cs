using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;
using System;
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
        room.ServerChangeScene(room.RoomScene);
    }

    public void HandleDisconnectClicked()
    {
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

using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private Room room;
    [SerializeField] private TMP_Text leftTeamWinsText;
    [SerializeField] private TMP_Text rightTeamWinsText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text waitingForHostText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void OnDestroy()
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
            gameOverPanel.SetActive(!gameOverPanel.activeSelf);
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

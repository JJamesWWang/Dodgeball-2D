using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
        MatchTracker.ClientMatchEnded += HandleMatchEnded;
    }

    private void OnDestroy()
    {
        MatchTracker.ClientMatchEnded -= HandleMatchEnded;
    }

    private void Start()
    {
        room = (Room)NetworkManager.singleton;
        if (NetworkServer.active)
            restartButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (NetworkServer.active)
            restartButton.gameObject.SetActive(true);
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            gameOverPanel.SetActive(!gameOverPanel.activeSelf);
    }

    private void HandleMatchEnded(bool isLeftTeamWin)
    {
        if (isLeftTeamWin)
            leftTeamWinsText.gameObject.SetActive(true);
        else
            rightTeamWinsText.gameObject.SetActive(true);
        waitingForHostText.gameObject.SetActive(true);
        gameOverPanel.SetActive(true);
    }

    public void HandleRestartClicked()
    {
        room.ServerChangeScene(room.RoomScene);
    }

    public void HandleDisconnectClicked()
    {
        if (NetworkServer.active)
            if (NetworkClient.isConnected)
                room.StopHost();
            else
                room.StopServer();
        else
            room.StopClient();
    }
}

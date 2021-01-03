using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void Start()
    {
        EventLogger.LogEvent("CLIENT: Player opened main menu.");
        if (Screen.fullScreen)
            Cursor.lockState = CursorLockMode.Confined;
    }

    public void HandleQuitClicked()
    {
        EventLogger.LogEvent("CLIENT: Game quit.");
        Application.Quit();
    }
}

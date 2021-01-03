using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void Start()
    {
        if (Screen.fullScreen)
            Cursor.lockState = CursorLockMode.Confined;
    }

    public void HandleQuitClicked()
    {
        Application.Quit();
    }
}

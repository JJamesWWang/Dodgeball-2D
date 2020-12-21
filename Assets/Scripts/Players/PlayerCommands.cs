using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCommands : NetworkBehaviour
{
    private PlayerMovement playerMovement = null;
    private Camera mainCamera = null;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            MoveTowards(mousePosition);
        }
    }

    private void MoveTowards(Vector2 mousePosition)
    {
        Vector2 point = mainCamera.ScreenToWorldPoint(mousePosition);
        playerMovement.CmdMoveTowards(point);
    }
}

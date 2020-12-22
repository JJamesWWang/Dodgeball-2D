using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCommands : NetworkBehaviour
{
    private Camera mainCamera = null;
    private PlayerMovement playerMovement = null;
    [SerializeField] private ThrowPowerBar throwPowerBarPrefab = null;
    private PlayerDodgeballThrower playerArm = null;

    private void Start()
    {
        mainCamera = Camera.main;
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerArm = GetComponentInParent<PlayerDodgeballThrower>();
    }

    private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            MoveTowards(mousePosition);
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartThrow();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseThrow(mousePosition);
        }
    }

    private void MoveTowards(Vector2 mousePosition)
    {
        Vector2 point = mainCamera.ScreenToWorldPoint(mousePosition);
        playerMovement.CmdMoveTowards(point);
    }

    private void StartThrow()
    {
        playerArm.CmdStartThrow();
        Instantiate(throwPowerBarPrefab, Vector3.zero, Quaternion.identity);
    }

    private void ReleaseThrow(Vector2 mousePosition)
    {
        Vector2 throwAtPoint = mainCamera.ScreenToWorldPoint(mousePosition);
        playerArm.CmdReleaseThrow(throwAtPoint);
    }
}

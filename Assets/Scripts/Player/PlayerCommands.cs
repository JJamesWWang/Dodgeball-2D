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
        playerMovement = GetComponent<PlayerMovement>();
        playerArm = GetComponent<PlayerDodgeballThrower>();
    }

    #region Client

    [ClientCallback]
    private void Update()
    {
        if (!Application.isFocused || !hasAuthority) { return; }
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

    [Client]
    private void MoveTowards(Vector2 mousePosition)
    {
        Vector2 point = mainCamera.ScreenToWorldPoint(mousePosition);
        playerMovement.CmdMoveTowards(point);
        playerMovement.CliMoveTowards(point);
    }

    [Client]
    private void StartThrow()
    {
        playerArm.CmdStartThrow();
        Instantiate(throwPowerBarPrefab, Vector3.zero, Quaternion.identity);
    }

    [Client]
    private void ReleaseThrow(Vector2 mousePosition)
    {
        Vector2 throwAtPoint = mainCamera.ScreenToWorldPoint(mousePosition);
        playerArm.CmdReleaseThrow(throwAtPoint);
    }

    #endregion
}

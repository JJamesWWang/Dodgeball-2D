using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCommands : NetworkBehaviour
{
    private Camera mainCamera = null;
    private PlayerMovement playerMovement = null;
    [SerializeField] private ThrowPowerBar throwPowerBarPrefab = null;
    private PlayerArm playerArm = null;

    private void Start()
    {
        mainCamera = Camera.main;
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerArm = GetComponentInParent<PlayerArm>();
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            MoveTowards(mousePosition);
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartThrow();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseThrow();
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

    private void ReleaseThrow()
    {
        playerArm.CmdReleaseThrow();
    }
}

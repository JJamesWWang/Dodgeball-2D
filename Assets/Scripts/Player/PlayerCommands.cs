using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

// Methods: TargetSetInputEnabled
public class PlayerCommands : NetworkBehaviour
{
    private bool inputEnabled = false;
    private Camera mainCamera = null;
    private PlayerMovement playerMovement;
    private PlayerArm playerArm;
    [SerializeField] private ThrowPowerBar throwPowerBarPrefab = null;
    private ThrowPowerBar throwPowerBar;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerArm = GetComponent<PlayerArm>();
        mainCamera = Camera.main;
    }

    #region Client

    [ClientCallback]
    private void Update()
    {
        if (!Application.isFocused || !hasAuthority || !inputEnabled) { return; }
        HandleInput();
    }

    [Client]
    private void HandleInput()
    {
        CheckMoveTowards();
        CheckStartThrow();
        CheckReleaseThrow();
    }

    [Client]
    private void CheckMoveTowards()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 point = mainCamera.ScreenToWorldPoint(mousePosition);
        MoveTowards(point);
    }

    [Client]
    private void MoveTowards(Vector2 point)
    {
        playerMovement.CmdMoveTowards(point);
    }

    [Client]
    private void CheckStartThrow()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) { return; }
        StartThrow();
    }

    [Client]
    private void StartThrow()
    {
        playerArm.CmdStartThrow();
        throwPowerBar = Instantiate(throwPowerBarPrefab, Vector3.zero, Quaternion.identity);
    }

    [Client]
    private void CheckReleaseThrow()
    {
        if (!Mouse.current.leftButton.wasReleasedThisFrame) { return; }
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 throwAtPoint = mainCamera.ScreenToWorldPoint(mousePosition);
        ReleaseThrow(throwAtPoint);
    }

    [Client]
    private void ReleaseThrow(Vector2 throwAtPoint)
    {
        playerArm.CmdReleaseThrow(throwAtPoint);
    }

    [TargetRpc]
    public void TargetSetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (throwPowerBar != null)
            Destroy(throwPowerBar.gameObject);
    }

    #endregion
}

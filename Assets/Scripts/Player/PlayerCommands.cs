using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCommands : NetworkBehaviour
{
    private bool inputEnabled = false;
    private Camera mainCamera = null;
    private PlayerMovement playerMovement;
    private PlayerArm playerArm;
    private ThrowPowerBar throwPowerBar;
    [SerializeField] private ThrowPowerBar throwPowerBarPrefab = null;

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
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (Mouse.current.rightButton.wasPressedThisFrame)
            MoveTowards(mousePosition);

        if (Mouse.current.leftButton.wasPressedThisFrame)
            StartThrow();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            ReleaseThrow(mousePosition);
    }

    [Client]
    private void MoveTowards(Vector2 mousePosition)
    {
        Vector2 point = mainCamera.ScreenToWorldPoint(mousePosition);
        playerMovement.CmdMoveTowards(point);
    }

    [Client]
    private void StartThrow()
    {
        playerArm.CmdStartThrow();
        throwPowerBar = Instantiate(throwPowerBarPrefab, Vector3.zero, Quaternion.identity);
    }

    [Client]
    private void ReleaseThrow(Vector2 mousePosition)
    {
        Vector2 throwAtPoint = mainCamera.ScreenToWorldPoint(mousePosition);
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

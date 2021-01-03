using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

// Methods: SetServerInputEnabled
public class PlayerCommands : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleServerInputEnabledUpdated))]
    private bool isServerInputEnabled = false;
    private bool isClientInputEnabled = true;
    private Camera mainCamera = null;
    private PlayerMovement playerMovement;
    private PlayerArm playerArm;
    [SerializeField] private ThrowPowerBar throwPowerBarPrefab = null;
    private ThrowPowerBar throwPowerBar;
    private CommandLogger commandLogger;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerArm = GetComponent<PlayerArm>();
        mainCamera = Camera.main;
        commandLogger = CommandLogger.singleton;
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    #region Server

    [Server]
    public void SetServerInputEnabled(bool enabled)
    {
        isServerInputEnabled = enabled;
    }

    #endregion

    #region Client

    [ClientCallback]
    private void Update()
    {
        if (!Application.isFocused || !hasAuthority ||
            !isServerInputEnabled || !isClientInputEnabled) { return; }
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
    private void MoveTowards(Vector2 destinationPoint)
    {
        playerMovement.CmdMoveTowards(destinationPoint);
        commandLogger.LogCommand($"Player wants to move to destination point {destinationPoint}.");
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
        throwPowerBar = Utils.InstantiateOffScreen(throwPowerBarPrefab);
        commandLogger.LogCommand("Player wants to start throw.");
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
        commandLogger.LogCommand($"Player wants to release throw at point {throwAtPoint}.");
    }

    [Client]
    private void HandleServerInputEnabledUpdated(bool _oldServerInputEnabled, bool _newServerInputEnabled)
    {
        DestroyThrowPowerBar();
    }

    private void DestroyThrowPowerBar()
    {
        if (throwPowerBar != null)
            Destroy(throwPowerBar.gameObject);
    }

    [ClientCallback]
    private void SubscribeEvents()
    {
        GameOverUI.ClientGameOverUIToggled += HandleGameOverUIToggled;
    }

    [Client]
    private void HandleGameOverUIToggled(bool isToggledOn)
    {
        isClientInputEnabled = !isToggledOn;
        DestroyThrowPowerBar();
    }

    [ClientCallback]
    private void UnsubscribeEvents()
    {
        GameOverUI.ClientGameOverUIToggled -= HandleGameOverUIToggled;
    }

    #endregion
}

using UnityEngine;
using Mirror;
using System;

public class PlayerConnection : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleTeamUpdated))]
    [SerializeField] private bool isLeftTeam;
    [SyncVar(hook = nameof(HandleUsernameUpdated))]
    [SerializeField] private string username;

    public bool IsLeftTeam { get { return isLeftTeam; } }
    public bool IsRightTeam { get { return !isLeftTeam; } }
    public string Username { get { return username; } }

    public static event Action ClientPlayerInfoUpdated;
    public static event Action<PlayerConnection> ClientPlayerSpawned;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    #region Server

    [Command]
    public void CmdSetUsername(string name)
    {
        if (string.IsNullOrEmpty(name)) { return; }
        SetUsername(name);
    }

    [Server]
    public void SetIsLeftTeam(bool value)
    {
        isLeftTeam = value;
    }

    [Server]
    public void SetUsername(string name)
    {
        username = name;
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }
        DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        networkManager.PlayerConnections.Add(this);
    }

    public override void OnStartLocalPlayer()
    {
        ClientPlayerSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (NetworkServer.active) { return; }
        DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        networkManager.PlayerConnections.Remove(this);
    }

    private void HandleTeamUpdated(bool _oldIsLeftTeam, bool _newIsLeftTeam)
    {
        ClientPlayerInfoUpdated?.Invoke();
    }

    private void HandleUsernameUpdated(string _oldName, string _newName)
    {
        ClientPlayerInfoUpdated?.Invoke();
    }

    #endregion

}

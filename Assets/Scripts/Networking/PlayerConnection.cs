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

    public static event Action<uint, string, object> ClientPlayerInfoUpdated;
    public static event Action<PlayerConnection> ClientPlayerConnected;
    public static event Action<PlayerConnection> ClientLocalPlayerConnected;
    public static event Action<PlayerConnection> ClientPlayerDisconnected;

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

    [Command]
    public void CmdSetIsLeftTeam(bool value)
    {
        if (GameState.Instance.IsInPlay) { return; }
        SetIsLeftTeam(value);
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
        if (!NetworkServer.active)
        {
            DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
            networkManager.PlayerConnections.Add(this);
        }
        ClientPlayerConnected?.Invoke(this);
    }

    public override void OnStartLocalPlayer()
    {
        ClientLocalPlayerConnected?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!NetworkServer.active)
        {
            DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
            networkManager.PlayerConnections.Remove(this);
        }
        ClientPlayerDisconnected?.Invoke(this);
    }

    [Client]
    private void HandleTeamUpdated(bool _oldIsLeftTeam, bool newIsLeftTeam)
    {
        ClientPlayerInfoUpdated?.Invoke(netId, nameof(IsLeftTeam), newIsLeftTeam);
    }

    [Client]
    private void HandleUsernameUpdated(string _oldName, string newName)
    {
        ClientPlayerInfoUpdated?.Invoke(netId, nameof(Username), newName);
    }

    #endregion

}

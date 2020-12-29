using System;
using Mirror;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleTeamUpdated))]
    [SerializeField] private bool isLeftTeam;
    [SyncVar(hook = nameof(HandleUsernameUpdated))]
    [SerializeField] private string username;

    public bool IsLeftTeam { get { return isLeftTeam; } }
    public bool IsRightTeam { get { return !isLeftTeam; } }
    public string Username { get { return username; } }

    public static event Action<uint, string, object> ClientPlayerInfoUpdated;

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

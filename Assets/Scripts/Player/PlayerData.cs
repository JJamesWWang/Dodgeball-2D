using UnityEngine;
using Mirror;
using System;

// Properties: IsLeftTeam, IsRightTeam, Username, IsSpectator
// Events: ClientPlayerDataUpdated
// Methods: CmdSetUsername, CmdSetIsLeftTeam, CmdSetIsSpectator, [Server] SetUsername, [Server] SetIsLeftTeam, [Server] SetIsSpectator
// Note: When a team is updated, IsLeftTeam will be used as the property name.
public class PlayerData : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleTeamUpdated))]
    private bool isLeftTeam;
    [SyncVar(hook = nameof(HandleUsernameUpdated))]
    private string username;
    // Temporarily serialized for debugging purposes
    [SyncVar(hook = nameof(HandleSpectatorUpdated))]
    [SerializeField] private bool isSpectator = false;

    public bool IsLeftTeam { get { return isLeftTeam; } }
    public bool IsRightTeam { get { return !isLeftTeam; } }
    public string Username { get { return username; } }
    public bool IsSpectator { get { return isSpectator; } }

    /// <summary> uint: connectionNetId, string: propertyName, object: propertyValue  </summary>
    public static event Action<uint, string, object> ClientPlayerDataUpdated;

    #region Server

    [Command]
    public void CmdSetIsLeftTeam(bool value)
    {
        SetIsLeftTeam(value);
    }

    [Command]
    public void CmdSetUsername(string name)
    {
        if (string.IsNullOrEmpty(name)) { return; }
        SetUsername(name);
    }

    [Command]
    public void CmdSetIsSpectator(bool value)
    {
        SetIsSpectator(value);
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

    [Server]
    public void SetIsSpectator(bool value)
    {
        isSpectator = value;
    }

    #endregion

    #region Client

    [Client]
    private void HandleTeamUpdated(bool _oldIsLeftTeam, bool newIsLeftTeam)
    {
        ClientPlayerDataUpdated?.Invoke(netId, nameof(IsLeftTeam), newIsLeftTeam);
    }

    [Client]
    private void HandleUsernameUpdated(string _oldName, string newName)
    {
        ClientPlayerDataUpdated?.Invoke(netId, nameof(Username), newName);
    }

    [Client]
    private void HandleSpectatorUpdated(bool _oldIsSpectator, bool newIsSpectator)
    {
        ClientPlayerDataUpdated?.Invoke(netId, nameof(IsSpectator), newIsSpectator);
    }

    #endregion
}

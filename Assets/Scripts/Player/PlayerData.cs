using UnityEngine;
using Mirror;
using System;

public enum Team
{
    Left = 0,
    Right = 1,
    Spectator
}

// Properties: Team, IsLeftTeam, IsRightTeam, IsSpectator, Username
// Events: ClientPlayerDataUpdated
// Methods: CmdSetUsername, CmdSetTeam, [Server] SetTeam, [Server] SetUsername
public class PlayerData : NetworkBehaviour
{
    // Temporarily serialized for debugging purposes
    [SyncVar(hook = nameof(HandleTeamUpdated))]
    [SerializeField] private Team team;
    [SyncVar(hook = nameof(HandleUsernameUpdated))]
    [SerializeField] private string username;

    public Team Team { get { return team; } }
    public bool IsLeftTeam { get { return team == Team.Left; } }
    public bool IsRightTeam { get { return team == Team.Right; } }
    public bool IsSpectator { get { return team == Team.Spectator; } }
    public string Username { get { return username; } }

    /// <summary> uint: connectionNetId, string: propertyName, object: propertyValue  </summary>
    public static event Action<uint, string, object> ClientPlayerDataUpdated;

    #region Server

    [Command]
    public void CmdSetTeam(Team team)
    {
        SetTeam(team);
    }

    [Command]
    public void CmdSetUsername(string name)
    {
        if (string.IsNullOrEmpty(name)) { return; }
        SetUsername(name);
    }

    [Server]
    public void SetTeam(Team team)
    {
        this.team = team;
    }

    [Server]
    public void SetUsername(string name)
    {
        username = name;
    }

    #endregion

    #region Client

    [Client]
    private void HandleTeamUpdated(Team _oldTeam, Team newTeam)
    {
        ClientPlayerDataUpdated?.Invoke(netId, nameof(Team), newTeam);
    }

    [Client]
    private void HandleUsernameUpdated(string _oldName, string newName)
    {
        ClientPlayerDataUpdated?.Invoke(netId, nameof(Username), newName);
    }

    #endregion
}

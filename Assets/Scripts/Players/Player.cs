using Mirror;
using System;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private bool isLeftTeam;

    public bool IsLeftTeam { get { return isLeftTeam; } }

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (NetworkServer.active) { return; }
        DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        networkManager.Players.Add(this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (NetworkServer.active) { return; }
        DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        networkManager.Players.Remove(this);
    }

    #endregion

    #region Server

    [Server]
    public void SetIsLeftTeam(bool value)
    {
        isLeftTeam = value;
    }

    #endregion

    public override bool Equals(object other)
    {
        if (!(other is Player)) { return false; }
        return connectionToClient.connectionId == ((NetworkBehaviour)other).connectionToClient.connectionId;
    }

    public override int GetHashCode()
    {
        return connectionToClient.connectionId.ToString().GetHashCode();
    }

}

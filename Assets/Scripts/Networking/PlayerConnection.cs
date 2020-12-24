using UnityEngine;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private bool isLeftTeam;

    public bool IsLeftTeam { get { return isLeftTeam; } }
    public bool IsRightTeam { get { return !isLeftTeam; } }

    #region Client

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }
        DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        networkManager.PlayerConnections.Add(this);
    }

    public override void OnStopClient()
    {
        if (NetworkServer.active) { return; }
        DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        networkManager.PlayerConnections.Remove(this);
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
        if (!(other is PlayerConnection)) { return false; }
        return connectionToClient.connectionId == ((NetworkBehaviour)other).connectionToClient.connectionId;
    }

    public override int GetHashCode()
    {
        return connectionToClient.connectionId.ToString().GetHashCode();
    }

}

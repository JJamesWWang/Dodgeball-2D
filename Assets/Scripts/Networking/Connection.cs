using UnityEngine;
using Mirror;
using System;

// Properties: PlayerData
// Events: ClientConnectionConnected, ClientLocalConnectionConnected, ClientConnectionDisconnected
public class Connection : NetworkRoomPlayer
{
    private Room room;
    public static Connection LocalConnection { get; private set; }
    public PlayerData PlayerData { get; private set; }
    public static event Action<Connection> ClientConnectionStarted;
    public static event Action<Connection> ClientLocalConnectionStarted;
    public static event Action<Connection> ClientConnectionStopped;

    private void Awake()
    {
        PlayerData = GetComponent<PlayerData>();
    }

    #region Server

    public override void OnStartServer()
    {
        room = (Room)NetworkManager.singleton;
        if (!NetworkClient.isConnected)
            room.AddConnection(this);
    }

    public override void OnStopServer()
    {
        if (!NetworkClient.isConnected)
            room.RemoveConnection(this);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        room = (Room)NetworkManager.singleton;
        room.AddConnection(this);
        ClientConnectionStarted?.Invoke(this);
    }

    public override void OnStartAuthority()
    {
        LocalConnection = this;
    }

    public override void OnStartLocalPlayer()
    {
        ClientLocalConnectionStarted?.Invoke(this);
    }

    public override void OnStopClient()
    {
        room.RemoveConnection(this);
        if (hasAuthority)
            LocalConnection = null;
        ClientConnectionStopped?.Invoke(this);
    }

    #endregion

}

using UnityEngine;
using Mirror;
using System;

// Properties: PlayerData
// Events: ClientConnected, ClientLocalConnected, ClientDisconnected
public class Connection : NetworkRoomPlayer
{
    private Room room;
    public static Connection LocalConnection { get; private set; }
    public PlayerData PlayerData { get; private set; }
    public static event Action<Connection> ClientConnected;
    public static event Action<Connection> ClientLocalConnected;
    public static event Action<Connection> ClientDisconnected;

    private void Awake()
    {
        PlayerData = GetComponent<PlayerData>();
    }

    #region Server

    public override void OnStartServer()
    {
        room = (Room)NetworkManager.singleton;
        room.AddConnection(this);
    }

    public override void OnStopServer()
    {
        room.RemoveConnection(this);
        if (NetworkClient.active)
            ClientDisconnected?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        room = (Room)NetworkManager.singleton;
        if (!NetworkServer.active)
            room.AddConnection(this);
        ClientConnected?.Invoke(this);
    }

    public override void OnStartAuthority()
    {
        LocalConnection = this;
    }

    public override void OnStartLocalPlayer()
    {
        ClientLocalConnected?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!NetworkServer.active)
            room.RemoveConnection(this);
        if (hasAuthority)
            LocalConnection = null;
        ClientDisconnected?.Invoke(this);
    }

    #endregion

}

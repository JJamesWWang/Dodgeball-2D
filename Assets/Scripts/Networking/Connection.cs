using UnityEngine;
using Mirror;
using System;

public class Connection : NetworkRoomPlayer
{
    public static event Action<Connection> ClientConnected;
    public static event Action<Connection> ClientLocalConnected;
    public static event Action<Connection> ClientDisconnected;

    #region Client

    public override void OnStartClient()
    {
        if (!NetworkServer.active)
        {
            Room room = (Room)NetworkManager.singleton;
            room.Connections.Add(this);
        }
        ClientConnected?.Invoke(this);
    }

    public override void OnStartLocalPlayer()
    {
        ClientLocalConnected?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!NetworkServer.active)
        {
            Room room = (Room)NetworkManager.singleton;
            room.Connections.Remove(this);
        }
        ClientDisconnected?.Invoke(this);
    }

    #endregion

}

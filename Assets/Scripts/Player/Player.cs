using Mirror;
using System;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private uint connectionNetId;

    public uint ConnectionNetId { get { return connectionNetId; } set { connectionNetId = value; } }
    public Connection Connection { get; private set; }
    public PlayerData Data { get; private set; }

    public static event Action<Player> ServerPlayerHit;
    public static event Action<Player> ClientPlayerSpawned;

    #region Server

    [Server]
    public void SetConnection(Connection connection)
    {
        Connection = connection;
        ConnectionNetId = connection.netId;
        Data = connection.GetComponent<PlayerData>();
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        Room room = (Room)NetworkManager.singleton;
        foreach (Connection connection in room.Connections)
            if (connection.netId == ConnectionNetId)
            {
                Connection = connection;
                Data = connection.GetComponent<PlayerData>();
                break;
            }

        ClientPlayerSpawned?.Invoke(this);
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D other)
    {
        ServerPlayerHit?.Invoke(this);
    }

    #endregion Client

}

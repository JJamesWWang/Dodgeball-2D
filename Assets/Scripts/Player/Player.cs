using Mirror;
using System;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Temporarily serialized for debugging purposes
    [SyncVar]
    [SerializeField] private uint connectionNetId;
    [SerializeField] private PlayerConnection connection;

    public uint ConnectionNetId { get { return connectionNetId; } }
    public PlayerConnection Connection { get { return connection; } }

    public static event Action<Player> ClientPlayerSpawned;

    #region Server

    [Server]
    public void SetConnectionNetId(uint id)
    {
        connectionNetId = id;
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        DodgeballNetworkManager dodgeballNetworkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        foreach (PlayerConnection playerConnection in dodgeballNetworkManager.PlayerConnections)
            if (playerConnection.netId == connectionNetId)
            {
                connection = playerConnection;
                break;
            }

        ClientPlayerSpawned?.Invoke(this);
    }

    #endregion Client

}

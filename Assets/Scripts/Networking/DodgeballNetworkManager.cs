using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class DodgeballNetworkManager : NetworkManager
{
    public List<PlayerConnection> PlayerConnections { get; } = new List<PlayerConnection>();

    public static event Action<NetworkConnection> ClientConnected;
    public static event Action<NetworkConnection> ClientDisconnected;

    #region Server

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        PlayerConnection playerConnection = Instantiate(playerPrefab).GetComponent<PlayerConnection>();
        SetUpPlayerConnection(playerConnection);
        NetworkServer.AddPlayerForConnection(conn, playerConnection.gameObject);
    }

    [Server]
    private void SetUpPlayerConnection(PlayerConnection playerConnection)
    {
        PlayerConnections.Add(playerConnection);
        playerConnection.SetIsLeftTeam(PlayerConnections.Count % 2 == 1);
        playerConnection.SetUsername($"Player {PlayerConnections.Count}");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        PlayerConnection playerConnection = conn.identity.GetComponent<PlayerConnection>();
        PlayerConnections.Remove(playerConnection);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        PlayerConnections.Clear();
    }

    #endregion

    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientConnected?.Invoke(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
        ClientDisconnected?.Invoke(conn);
    }

    #endregion
}

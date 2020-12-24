using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class DodgeballNetworkManager : NetworkManager
{
    public List<PlayerConnection> PlayerConnections { get; } = new List<PlayerConnection>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        PlayerConnection playerConnection = Instantiate(playerPrefab).GetComponent<PlayerConnection>();
        NetworkServer.AddPlayerForConnection(conn, playerConnection.gameObject);
        SetUpPlayerConnection(playerConnection);
    }

    [Server]
    private void SetUpPlayerConnection(PlayerConnection playerConnection)
    {
        PlayerConnections.Add(playerConnection);
        playerConnection.SetIsLeftTeam(PlayerConnections.Count % 2 == 1);
        if (playerConnection.IsRightTeam)
            playerConnection.transform.Rotate(0f, 0f, 180f);
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

}

using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class DodgeballNetworkManager : NetworkManager
{
    public HashSet<Player> Players { get; } = new HashSet<Player>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Player player = conn.identity.GetComponent<Player>();
        Players.Add(player);
        player.SetIsLeftTeam(Players.Count % 2 == 1);
        if (!player.IsLeftTeam)
            player.transform.Rotate(0f, 0f, 180f);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Players.Remove(conn.identity.GetComponent<Player>());
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Players.Clear();
    }

}

using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class DodgeballNetworkManager : NetworkManager
{
    public List<Player> Players { get; } = new List<Player>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Player player = conn.identity.GetComponent<Player>();
        Players.Add(player);
        player.SetIsLeftTeam(Players.Count % 2 == 1);
        if (!player.IsLeftTeam)
            player.transform.Rotate(0f, 0f, 180f);
    }
}

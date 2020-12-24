using UnityEngine;
using Mirror;
using System;

public class RoundTracker : NetworkBehaviour
{
    private PlayerTracker playerTracker;

    public static event Action<bool> ServerRoundEnded;

    [ServerCallback]
    private void Start()
    {
        playerTracker = GetComponentInParent<PlayerTracker>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerTracker.ServerPlayerEliminated += HandlePlayerEliminated;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        PlayerTracker.ServerPlayerEliminated -= HandlePlayerEliminated;
    }

    private void HandlePlayerEliminated(Player player)
    {
        if (playerTracker.LeftTeamPlayers.Count == 0 || playerTracker.RightTeamPlayers.Count == 0)
            ServerRoundEnded?.Invoke(player.Connection.IsRightTeam);
    }

    public void StartRound()
    {
        playerTracker.DespawnPlayers();
        playerTracker.SpawnPlayers();
    }
}

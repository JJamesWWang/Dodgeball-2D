using UnityEngine;
using Mirror;
using System;

public class RoundTracker : NetworkBehaviour
{
    private PlayerTracker playerTracker;
    private DodgeballTracker dodgeballTracker;

    public static event Action<bool> ServerRoundEnded;

    [ServerCallback]
    private void Start()
    {
        playerTracker = GetComponent<PlayerTracker>();
        dodgeballTracker = GetComponent<DodgeballTracker>();
    }

    #region Server

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

    [Server]
    private void HandlePlayerEliminated(Player player)
    {
        if (playerTracker.LeftTeamPlayers.Count == 0 || playerTracker.RightTeamPlayers.Count == 0)
            ServerRoundEnded?.Invoke(player.Connection.IsRightTeam);
    }

    [Server]
    public void StartRound()
    {
        playerTracker.DespawnPlayers();
        dodgeballTracker.DespawnDodgeballs();
        playerTracker.SpawnPlayers();
    }

    #endregion Server
}

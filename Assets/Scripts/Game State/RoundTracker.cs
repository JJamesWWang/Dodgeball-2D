using UnityEngine;
using Mirror;
using System;
using System.Collections;

public class RoundTracker : NetworkBehaviour
{
    private PlayerTracker playerTracker;
    private DodgeballTracker dodgeballTracker;
    [SerializeField] private float timeBetweenRounds = 3f;

    public static event Action<bool> ServerRoundEnded;
    public static event Action<float> ClientCountdownStarted;

    [ServerCallback]
    private void Awake()
    {
        playerTracker = GetComponent<PlayerTracker>();
        dodgeballTracker = GetComponent<DodgeballTracker>();
    }

    #region Server

    public override void OnStartServer()
    {
        PlayerTracker.ServerPlayerEliminated += HandlePlayerEliminated;
    }

    public override void OnStopServer()
    {
        PlayerTracker.ServerPlayerEliminated -= HandlePlayerEliminated;
    }

    [Server]
    private void HandlePlayerEliminated(Player player)
    {
        if (playerTracker.LeftTeamPlayers.Count == 0 || playerTracker.RightTeamPlayers.Count == 0)
            ServerRoundEnded?.Invoke(player.IsRightTeam);
    }

    [Server]
    public void StartRound()
    {
        playerTracker.DespawnPlayers();
        dodgeballTracker.DespawnDodgeballs();
        StartCoroutine(SpawnPlayersAfterWaitTime());
    }

    [Server]
    private IEnumerator SpawnPlayersAfterWaitTime()
    {
        InvokeCountdownStarted();
        yield return new WaitForSeconds(timeBetweenRounds);
        playerTracker.SpawnPlayers();
    }

    #endregion Server

    #region Client

    [ClientRpc]
    private void InvokeCountdownStarted()
    {
        ClientCountdownStarted?.Invoke(timeBetweenRounds);
    }

    #endregion
}

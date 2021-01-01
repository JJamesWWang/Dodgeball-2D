using UnityEngine;
using Mirror;
using System;
using System.Collections;

// Events: ServerRoundOver, ClientCountdownStarted
// Methods: [Server] StartRound
public class RoundTracker : NetworkBehaviour
{
    private PlayerTracker playerTracker;
    private DodgeballTracker dodgeballTracker;
    [SerializeField] private float timeBetweenRounds = 3f;

    /// <summary> bool: isLeftTeamWin </summary>
    public static event Action<bool> ServerRoundOver;
    /// <summary> float: timeBetweenRounds </summary>
    public static event Action<float> ClientCountdownStarted;

    private void Awake()
    {
        playerTracker = GetComponent<PlayerTracker>();
        dodgeballTracker = GetComponent<DodgeballTracker>();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
        StopAllCoroutines();
    }

    #region Server

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

    [ServerCallback]
    private void SubscribeEvents()
    {
        PlayerTracker.ServerPlayerEliminated += HandlePlayerEliminated;
    }

    [Server]
    private void HandlePlayerEliminated(Player player)
    {
        if (IsRoundOver())
            ServerRoundOver?.Invoke(player.IsRightTeam);
    }

    [Server]
    private bool IsRoundOver()
    {
        return playerTracker.LeftTeamPlayers.Count == 0 ||
                playerTracker.RightTeamPlayers.Count == 0;
    }

    [ServerCallback]
    private void UnsubscribeEvents()
    {
        PlayerTracker.ServerPlayerEliminated -= HandlePlayerEliminated;
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

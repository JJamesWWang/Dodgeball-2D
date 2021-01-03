using UnityEngine;
using Mirror;
using System;
using System.Collections;

// Events: ServerRoundOver, ClientCountdownStarted, ClientRoundOver
// Methods: [Server] StartRound
public class RoundTracker : NetworkBehaviour
{
    private PlayerTracker playerTracker;
    private DodgeballTracker dodgeballTracker;
    [SerializeField] private float timeToShowWinner = 2f;
    [SerializeField] private float timeToStartRound = 3f;

    /// <summary> bool: isLeftTeamWin </summary>
    public static event Action<bool> ServerRoundEnded;
    /// <summary> float: timeToStartRound </summary>
    public static event Action<float> ClientCountdownStarted;
    public static event Action<bool> ClientRoundEnded;

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
        StartCoroutine(SpawnPlayers());
    }

    [Server]
    private IEnumerator SpawnPlayers()
    {
        yield return new WaitForSeconds(timeToShowWinner);
        playerTracker.SpawnPlayers();
        RpcInvokeCountdownStarted();
        playerTracker.SetPlayerInputEnabled(false);
        yield return new WaitForSeconds(timeToStartRound);
        playerTracker.SetPlayerInputEnabled(true);
    }

    [ServerCallback]
    private void SubscribeEvents()
    {
        PlayerTracker.ServerPlayerEliminated += HandlePlayerEliminated;
        MatchTracker.ServerMatchEnded += HandleMatchEnded;
    }

    [Server]
    private void HandlePlayerEliminated(Player player)
    {
        if (IsRoundOver())
        {
            // Invoke client first, otherwise UI will overlap
            RpcInvokeRoundOver(player.IsRightTeam);
            ServerRoundEnded?.Invoke(player.IsRightTeam);
        }
    }

    [Server]
    private bool IsRoundOver()
    {
        return playerTracker.LeftTeamActivePlayers.Count == 0 ||
                playerTracker.RightTeamActivePlayers.Count == 0;
    }

    [Server]
    private void HandleMatchEnded(bool _isLeftTeamWin)
    {
        StopAllCoroutines();
    }

    [ServerCallback]
    private void UnsubscribeEvents()
    {
        PlayerTracker.ServerPlayerEliminated -= HandlePlayerEliminated;
        MatchTracker.ServerMatchEnded -= HandleMatchEnded;
    }

    #endregion Server

    #region Client

    [ClientRpc]
    private void RpcInvokeCountdownStarted()
    {
        ClientCountdownStarted?.Invoke(timeToStartRound);
    }

    [ClientRpc]
    private void RpcInvokeRoundOver(bool isLeftTeamWin)
    {
        ClientRoundEnded?.Invoke(isLeftTeamWin);
    }

    #endregion
}

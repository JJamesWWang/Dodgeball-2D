using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class PlayerTracker : NetworkBehaviour
{
    // The following are temporarily serialized for debugging purposes
    [SerializeField] private List<Player> leftTeamPlayers = new List<Player>();
    [SerializeField] private List<Player> rightTeamPlayers = new List<Player>();

    public List<Player> LeftTeamPlayers { get { return leftTeamPlayers; } }
    public List<Player> RightTeamPlayers { get { return rightTeamPlayers; } }

    private DodgeballNetworkManager dodgeballNetworkManager;
    [SerializeField] private Player playerPrefab = null;

    public static event Action<Player> ServerPlayerEliminated;

    private void Start()
    {
        dodgeballNetworkManager = (DodgeballNetworkManager)NetworkManager.singleton;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Dodgeball.ServerPlayerHit += HandlePlayerHit;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Dodgeball.ServerPlayerHit -= HandlePlayerHit;
    }

    [Server]
    private void HandlePlayerHit(Player player)
    {
        if (player.Connection.IsLeftTeam)
            leftTeamPlayers.Remove(player);
        else
            rightTeamPlayers.Remove(player);
        EliminatePlayer(player);
    }

    [Server]
    private void EliminatePlayer(Player player)
    {
        ServerPlayerEliminated?.Invoke(player);
        Destroy(player.gameObject);
    }

    [Server]
    public void SpawnPlayers()
    {
        foreach (PlayerConnection playerConnection in dodgeballNetworkManager.PlayerConnections)
            SpawnPlayer(playerConnection);
    }

    [Server]
    private void SpawnPlayer(PlayerConnection playerConnection)
    {
        bool isLeftTeam = playerConnection.IsLeftTeam;
        Transform spawnPoint = Map.Instance.GetSpawnPoint(isLeftTeam);
        Player player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        player.SetConnection(playerConnection);
        if (isLeftTeam)
            leftTeamPlayers.Add(player);
        else
            rightTeamPlayers.Add(player);
        NetworkServer.Spawn(player.gameObject, playerConnection.connectionToClient);
    }

    [Server]
    public void DespawnPlayers()
    {
        foreach (Player player in leftTeamPlayers)
            Destroy(player.gameObject);
        foreach (Player player in rightTeamPlayers)
            Destroy(player.gameObject);

        leftTeamPlayers.Clear();
        rightTeamPlayers.Clear();
    }
}

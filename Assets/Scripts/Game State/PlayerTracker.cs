﻿using UnityEngine;
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

    #region Server

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
        NetworkServer.Destroy(player.gameObject);
        ServerPlayerEliminated?.Invoke(player);
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
        if (playerConnection.IsRightTeam)
            player.transform.Rotate(0f, 0f, 180f);
        player.SetConnectionNetId(playerConnection.netId);
        NetworkServer.Spawn(player.gameObject, playerConnection.connectionToClient);

        if (isLeftTeam)
            leftTeamPlayers.Add(player);
        else
            rightTeamPlayers.Add(player);
    }

    [Server]
    public void DespawnPlayers()
    {
        foreach (Player player in leftTeamPlayers)
            NetworkServer.Destroy(player.gameObject);
        foreach (Player player in rightTeamPlayers)
            NetworkServer.Destroy(player.gameObject);

        leftTeamPlayers.Clear();
        rightTeamPlayers.Clear();
    }

    #endregion
}

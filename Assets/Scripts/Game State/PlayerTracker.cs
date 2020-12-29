using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class PlayerTracker : NetworkBehaviour
{
    private Room room;
    public List<Player> LeftTeamPlayers { get; } = new List<Player>();
    public List<Player> RightTeamPlayers { get; } = new List<Player>();

    public static event Action<Player> ServerPlayerEliminated;

    private void Start()
    {
        room = (Room)NetworkManager.singleton;
    }

    #region Server

    public override void OnStartServer()
    {
        Player.ServerPlayerHit += HandlePlayerHit;
    }

    public override void OnStopServer()
    {
        Player.ServerPlayerHit -= HandlePlayerHit;
    }

    [Server]
    private void HandlePlayerHit(Player player)
    {
        if (player.Data.IsLeftTeam)
            LeftTeamPlayers.Remove(player);
        else
            RightTeamPlayers.Remove(player);
        EliminatePlayer(player);
    }

    [Server]
    private void EliminatePlayer(Player player)
    {
        // Temporarily set player to out of nowhere
        player.transform.position = new Vector2(5000, 0);
        ServerPlayerEliminated?.Invoke(player);
    }

    [Server]
    public void SpawnPlayers()
    {
        foreach (Player player in room.Players)
            SpawnPlayer(player);
    }

    [Server]
    private void SpawnPlayer(Player player)
    {
        var playerData = player.Data;
        bool isLeftTeam = playerData.IsLeftTeam;
        Transform spawnPoint = Map.Instance.GetSpawnPoint(isLeftTeam);
        player.transform.position = spawnPoint.position;
        player.transform.rotation = Quaternion.identity;
        if (playerData.IsRightTeam)
            player.transform.Rotate(0f, 0f, 180f);

        if (isLeftTeam)
            LeftTeamPlayers.Add(player);
        else
            RightTeamPlayers.Add(player);
    }

    [Server]
    public void DespawnPlayers()
    {
        // Temporarily set player to out of nowhere
        foreach (Player player in LeftTeamPlayers)
            player.transform.position = new Vector2(5000, 0);
        foreach (Player player in RightTeamPlayers)
            player.transform.position = new Vector2(5000, 0);

        LeftTeamPlayers.Clear();
        RightTeamPlayers.Clear();
    }

    #endregion
}

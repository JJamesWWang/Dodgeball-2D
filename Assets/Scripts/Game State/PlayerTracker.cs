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
    public void SpawnPlayers()
    {
        foreach (Player player in room.Players)
            SpawnPlayer(player);
    }

    [Server]
    private void SpawnPlayer(Player player)
    {
        player.transform.position = GetPlayerSpawnPoint(player);
        player.transform.localEulerAngles = GetPlayerSpawnRotation(player);
        player.EnableInput();
        AddPlayer(player);

    }

    private Vector2 GetPlayerSpawnPoint(Player player)
    {
        Transform spawnPoint = Map.Instance.GetSpawnPoint(player.IsLeftTeam);
        return spawnPoint.position;
    }

    private Vector3 GetPlayerSpawnRotation(Player player)
    {
        if (player.IsRightTeam)
            return new Vector3(0f, 0f, -180f);
        return Vector3.zero;
    }

    private void AddPlayer(Player player)
    {
        if (player.IsLeftTeam)
            LeftTeamPlayers.Add(player);
        else
            RightTeamPlayers.Add(player);
    }

    [Server]
    public void DespawnPlayers()
    {
        // Temporarily set player to out of nowhere
        foreach (Player player in LeftTeamPlayers)
            DespawnPlayer(player);
        foreach (Player player in RightTeamPlayers)
            DespawnPlayer(player);
        LeftTeamPlayers.Clear();
        RightTeamPlayers.Clear();
    }

    [Server]
    private void DespawnPlayer(Player player)
    {
        player.transform.position = new Vector2(5000, 0);
        player.DisableInput();
    }

    private void RemovePlayer(Player player)
    {
        if (player.IsLeftTeam)
            LeftTeamPlayers.Remove(player);
        else
            RightTeamPlayers.Remove(player);
    }

    [Server]
    private void HandlePlayerHit(Player player)
    {
        EliminatePlayer(player);
    }

    [Server]
    private void EliminatePlayer(Player player)
    {
        // Temporarily set player to out of nowhere
        DespawnPlayer(player);
        RemovePlayer(player);
        ServerPlayerEliminated?.Invoke(player);
    }

    #endregion
}

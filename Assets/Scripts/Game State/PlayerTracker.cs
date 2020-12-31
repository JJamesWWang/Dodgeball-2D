using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Properties: LeftTeamPlayers, RightTeamPlayers
// Events: ServerPlayerEliminated, ServerATeamLeft
// Methods: [Server] SpawnPlayers, [Server] DespawnPlayers
public class PlayerTracker : NetworkBehaviour
{
    private Room room;
    // Temporarily serialized for debugging purposes
    [SerializeField] private List<Player> leftTeamPlayers = new List<Player>();
    [SerializeField] private List<Player> rightTeamPlayers = new List<Player>();

    public ReadOnlyCollection<Player> LeftTeamPlayers { get { return leftTeamPlayers.AsReadOnly(); } }
    public ReadOnlyCollection<Player> RightTeamPlayers { get { return rightTeamPlayers.AsReadOnly(); } }

    public static event Action<Player> ServerPlayerEliminated;
    /// <summary> bool: isLeftTeam </summary>
    public static event Action<bool> ServerATeamLeft;

    private void Start()
    {
        room = (Room)NetworkManager.singleton;
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    #region Server

    [Server]
    public void SpawnPlayers()
    {
        foreach (Player player in room.Players)
            if (!player.IsSpectator)
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

    [Server]
    private Vector2 GetPlayerSpawnPoint(Player player)
    {
        Transform spawnPoint = Map.Instance.GetSpawnPoint(player.IsLeftTeam);
        return spawnPoint.position;
    }

    [Server]
    private Vector3 GetPlayerSpawnRotation(Player player)
    {
        if (player.IsRightTeam)
            return new Vector3(0f, 0f, -180f);
        return Vector3.zero;
    }

    [Server]
    private void AddPlayer(Player player)
    {
        if (player.IsLeftTeam)
            leftTeamPlayers.Add(player);
        else
            rightTeamPlayers.Add(player);
    }

    [Server]
    public void DespawnPlayers()
    {
        // Temporarily set player to out of nowhere
        foreach (Player player in LeftTeamPlayers)
            DespawnPlayer(player);
        foreach (Player player in RightTeamPlayers)
            DespawnPlayer(player);
        leftTeamPlayers.Clear();
        rightTeamPlayers.Clear();
    }

    [Server]
    private void DespawnPlayer(Player player)
    {
        player.transform.position = new Vector2(5000, 0);
        player.DisableInput();
    }

    [Server]
    private void SubscribeEvents()
    {
        Player.ServerPlayerHit += HandlePlayerHit;
        Player.ServerPlayerDisconnected += HandlePlayerDisconnected;
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

    [Server]
    private void RemovePlayer(Player player)
    {
        if (player.IsLeftTeam)
            leftTeamPlayers.Remove(player);
        else
            rightTeamPlayers.Remove(player);
    }

    [Server]
    private void HandlePlayerDisconnected(Player player)
    {

        if (IsATeamEmpty(out int leftTeamPlayerCount))
            ServerATeamLeft?.Invoke(leftTeamPlayerCount == 0);
        else
            EliminateDisconnectedPlayer(player);
    }

    [Server]
    private bool IsATeamEmpty(out int leftTeamPlayerCount)
    {
        leftTeamPlayerCount = CountPlayersOfTeam(Team.Left);
        int rightTeamPlayerCount = CountPlayersOfTeam(Team.Right);
        return leftTeamPlayerCount == 0 || rightTeamPlayerCount == 0;
    }

    [Server]
    private int CountPlayersOfTeam(Team team)
    {
        int playerCount = 0;
        foreach (Player player in room.Players)
            if (player.Team == team)
                playerCount += 1;
        return playerCount;
    }

    [Server]
    private void EliminateDisconnectedPlayer(Player player)
    {
        RemovePlayer(player);
        ServerPlayerEliminated?.Invoke(player);
    }

    [Server]
    private void UnsubscribeEvents()
    {
        Player.ServerPlayerHit -= HandlePlayerHit;
        Player.ServerPlayerDisconnected -= HandlePlayerDisconnected;
    }

    #endregion
}

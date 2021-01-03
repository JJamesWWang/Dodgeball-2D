using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Properties: ActivePlayers, LeftTeamActivePlayers, RightTeamActivePlayers
// Events: ServerPlayerEliminated, ServerATeamLeft
// Methods: [Server] SpawnPlayers, [Server] DespawnPlayers, [Server] EnablePlayerInput, [Server] DisablePlayerInput
public class PlayerTracker : NetworkBehaviour
{

    // Temporarily serialized for debugging purposes
    [SerializeField] private List<Player> activePlayers = new List<Player>();
    [SerializeField] private List<Player> leftTeamActivePlayers = new List<Player>();
    [SerializeField] private List<Player> rightTeamActivePlayers = new List<Player>();
    private Room room;
    private Map map;

    /// <summary> Players that haven't been eliminated. </summary>
    public ReadOnlyCollection<Player> ActivePlayers { get { return activePlayers.AsReadOnly(); } }
    public ReadOnlyCollection<Player> LeftTeamActivePlayers { get { return leftTeamActivePlayers.AsReadOnly(); } }
    public ReadOnlyCollection<Player> RightTeamActivePlayers { get { return rightTeamActivePlayers.AsReadOnly(); } }

    public static event Action<Player> ServerPlayerEliminated;
    /// <summary> bool: isLeftTeam </summary>
    public static event Action<bool> ServerATeamLeft;

    private void Start()
    {
        room = (Room)NetworkManager.singleton;
        map = FindObjectOfType<Map>();
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
        ClearNullActivePlayers();
        foreach (Player player in room.Players)
            if (!player.IsSpectator)
                SpawnPlayer(player);
    }

    // More of a safety check, theoretically this should never do anything.
    [Server]
    private void ClearNullActivePlayers()
    {
        foreach (Player player in activePlayers)
            RemoveNullActivePlayer(activePlayers, player);
        foreach (Player player in leftTeamActivePlayers)
            RemoveNullActivePlayer(leftTeamActivePlayers, player);
        foreach (Player player in rightTeamActivePlayers)
            RemoveNullActivePlayer(rightTeamActivePlayers, player);
    }

    [Server]
    private void RemoveNullActivePlayer(List<Player> playerList, Player player)
    {
        if (player != null) { return; }
        Debug.LogError("Active Player was null. Removed null Active Player.");
        playerList.Remove(player);
    }

    [Server]
    private void SpawnPlayer(Player player)
    {
        player.transform.position = GetPlayerSpawnPoint(player);
        player.transform.localEulerAngles = GetPlayerSpawnRotation(player);
        player.SetOnField(true);
        player.SetInputEnabled(true);
        AddPlayer(player);
    }

    [Server]
    private Vector2 GetPlayerSpawnPoint(Player player)
    {
        Transform spawnPoint = map.GetSpawnPoint(player.IsLeftTeam);
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
        activePlayers.Add(player);
        if (player.IsLeftTeam)
            leftTeamActivePlayers.Add(player);
        else if (player.IsRightTeam)
            rightTeamActivePlayers.Add(player);
    }

    [Server]
    public void SetPlayerInputEnabled(bool enabled)
    {
        ClearNullActivePlayers();
        foreach (Player player in ActivePlayers)
            player.SetInputEnabled(enabled);
    }

    [Server]
    public void DespawnPlayers()
    {
        ClearNullActivePlayers();
        foreach (Player player in new List<Player>(ActivePlayers))
            DespawnPlayer(player);
        leftTeamActivePlayers.Clear();
        rightTeamActivePlayers.Clear();
    }

    private void DespawnPlayer(Player player)
    {
        player.SetOnField(false);
        player.SetInputEnabled(false);
        RemovePlayer(player);
    }

    [Server]
    private void RemovePlayer(Player player)
    {
        activePlayers.Remove(player);
        if (player.IsLeftTeam)
            leftTeamActivePlayers.Remove(player);
        else
            rightTeamActivePlayers.Remove(player);
    }

    [ServerCallback]
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
        DespawnPlayer(player);
        ServerPlayerEliminated?.Invoke(player);
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

    [ServerCallback]
    private void UnsubscribeEvents()
    {
        Player.ServerPlayerHit -= HandlePlayerHit;
        Player.ServerPlayerDisconnected -= HandlePlayerDisconnected;
    }

    #endregion
}

using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Properties: Connections, Players
// Events: ClientConnected, ClientDisconnected
// Methods: AddConnection, RemoveConnection, AddPlayer, RemovePlayer, Disconnect
public class Room : NetworkRoomManager
{
    // Temporarily serialized for debugging purposes
    [SerializeField] private List<Connection> connections = new List<Connection>();
    [SerializeField] private List<Player> players = new List<Player>();

    public IReadOnlyCollection<Connection> Connections { get { CheckNullConnections(); return connections; } }
    public IReadOnlyCollection<Player> Players { get { CheckNullPlayers(); return players; } }

    public static event Action ClientConnected;
    public static event Action ClientDisconnected;

    #region General

    // Theoretically should never happen, more of a safety check
    private void CheckNullConnections()
    {
        foreach (var connection in new List<Connection>(connections))
            RemoveNullConnection(connection);
    }

    private void RemoveNullConnection(Connection connection)
    {
        if (connection != null) { return; }
        connections.Remove(connection);
        EventLogger.LogError("Connection was null. Removed null Connection.");
    }

    // Theoretically should never happen, more of a safety check
    private void CheckNullPlayers()
    {
        foreach (var player in new List<Player>(players))
            RemoveNullPlayer(player);
    }

    private void RemoveNullPlayer(Player player)
    {
        if (player != null) { return; }
        players.Remove(player);
        EventLogger.LogError("Player was null. Removed null Player.");
    }

    public void AddConnection(Connection connection)
    {
        connections.Add(connection);
    }

    public void RemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
    }

    public virtual void Disconnect()
    {
        if (NetworkServer.active)
            if (NetworkClient.isConnected)
                StopHost();
            else
                StopServer();
        else if (NetworkClient.isConnected)
            StopClient();
    }

    #endregion

    #region Server

    public override void OnRoomStartServer()
    {
        EventLogger.LogEvent("SERVER: Server started.");
    }

    public override void OnRoomStopServer()
    {
        EventLogger.LogEvent("SERVER: Server stopped.");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        OnRoomServerConnect(conn);
    }

    public override void OnRoomServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = OnRoomServerCreateRoomPlayer(conn);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
    {
        var connection = Instantiate(roomPlayerPrefab).GetComponent<Connection>();
        SetUpConnection(connection);
        return connection.gameObject;
    }

    [Server]
    private void SetUpConnection(Connection connection)
    {
        var playerData = connection.PlayerData;
        var playerNumber = Connections.Count + 1;
        SetPlayerTeam(playerData, playerNumber);
        playerData.SetUsername($"Player {playerNumber}");
    }

    [Server]
    private void SetPlayerTeam(PlayerData playerData, int playerNumber)
    {
        if (!IsSceneActive(RoomScene))
            playerData.SetTeam(Team.Spectator);
        else if (playerNumber % 2 == 1)
            playerData.SetTeam(Team.Left);
        else
            playerData.SetTeam(Team.Right);
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        return CreateGamePlayer(roomPlayer).gameObject;
    }

    [Server]
    private Player CreateGamePlayer(GameObject roomPlayer)
    {
        var connection = roomPlayer.GetComponent<Connection>();
        var player = Utils.InstantiateOffScreen(playerPrefab).GetComponent<Player>();
        player.SetConnection(connection);
        return player;
    }

    #endregion

    #region Client

    public override void OnRoomClientConnect(NetworkConnection conn)
    {
        ClientConnected?.Invoke();
    }

    public override void OnRoomClientDisconnect(NetworkConnection conn)
    {
        ClientDisconnected?.Invoke();
    }

    #endregion

}

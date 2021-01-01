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

    public ReadOnlyCollection<Connection> Connections { get { return connections.AsReadOnly(); } }
    public ReadOnlyCollection<Player> Players { get { return players.AsReadOnly(); } }

    public static event Action ClientConnected;
    public static event Action ClientDisconnected;

    #region General

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

    public void Disconnect()
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
        // Temporarily set player to out of nowhere
        var player = Instantiate(playerPrefab, new Vector3(5000f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
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

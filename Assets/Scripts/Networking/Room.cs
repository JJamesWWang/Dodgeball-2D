using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Properties: Connections, Players
// Methods: AddConnection, RemoveConnection, AddPlayer, RemovePlayer, Disconnect
public class Room : NetworkRoomManager
{
    [SerializeField] private float timeToWaitForAllPlayersToConnect = 5f;
    private List<Connection> connections = new List<Connection>();
    private List<Player> players = new List<Player>();

    public ReadOnlyCollection<Connection> Connections { get { return connections.AsReadOnly(); } }
    public ReadOnlyCollection<Player> Players { get { return players.AsReadOnly(); } }

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
        else
            StopClient();
    }

    #endregion

    #region Server

    public override void OnRoomStartServer()
    {
        SubscribeEvents();
    }

    public override void OnRoomStopServer()
    {
        UnsubscribeEvents();
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
        playerData.SetIsLeftTeam(playerNumber % 2 == 1);
        playerData.SetUsername($"Player {playerNumber}");
        if (!IsSceneActive(RoomScene))
            playerData.SetIsSpectator(true);
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

    [Server]
    private void SubscribeEvents()
    {
        GameState.ServerGameStateReady += HandleGameStateReady;
    }

    [Server]
    private void HandleGameStateReady()
    {
        StartCoroutine(StartGame());
    }

    [Server]
    private IEnumerator StartGame()
    {
        yield return WaitForAllPlayersToConnect();
        GameState.Instance.StartGame();
    }

    [Server]
    private IEnumerator WaitForAllPlayersToConnect()
    {
        int secondsPassed = 0;
        while (secondsPassed < timeToWaitForAllPlayersToConnect)
        {
            if (Players.Count == Connections.Count)
                break;
            secondsPassed += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    [Server]
    private void UnsubscribeEvents()
    {
        GameState.ServerGameStateReady -= HandleGameStateReady;
    }

    #endregion

}

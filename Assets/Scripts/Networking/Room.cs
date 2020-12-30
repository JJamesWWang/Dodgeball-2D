using System.Xml.Linq;
using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections.ObjectModel;

public class Room : NetworkRoomManager
{
    [SerializeField] private float timeToWaitForAllPlayersToConnect = 5f;
    private List<Connection> connections = new List<Connection>();
    private List<Player> players = new List<Player>();

    public ReadOnlyCollection<Connection> Connections { get { return connections.AsReadOnly(); } }
    public ReadOnlyCollection<Player> Players { get { return players.AsReadOnly(); } }

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


    #region Server

    public override void OnRoomStartServer()
    {
        GameState.ServerGameStateReady += HandleGameStateReady;
    }

    public override void OnRoomStopServer()
    {
        GameState.ServerGameStateReady -= HandleGameStateReady;
    }

    private void HandleGameStateReady()
    {
        StartCoroutine(WaitForAllPlayersToConnect());
        GameState.Instance.StartGame();
    }

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
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        return CreateGamePlayer(roomPlayer).gameObject;
    }

    private Player CreateGamePlayer(GameObject roomPlayer)
    {
        var connection = roomPlayer.GetComponent<Connection>();
        var playerData = connection.PlayerData;
        bool isLeftTeam = playerData.IsLeftTeam;
        Transform spawnPoint = Map.Instance.GetSpawnPoint(isLeftTeam);
        var player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Player>();
        if (playerData.IsRightTeam)
            player.transform.Rotate(0f, 0f, 180f);
        player.SetConnection(connection);
        return player;
    }

    public override void OnRoomServerDisconnect(NetworkConnection conn)
    {
    }

    public override void OnRoomStopHost()
    {
    }

    #endregion

    #region Client



    #endregion

}

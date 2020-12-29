using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Room : NetworkRoomManager
{
    [SerializeField] private float timeToWaitForAllPlayersToConnect = 5f;
    public List<Connection> Connections { get; } = new List<Connection>();
    public List<Player> Players { get; } = new List<Player>();

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
        Connections.Add(connection);
        var playerData = connection.GetComponent<PlayerData>();
        playerData.SetIsLeftTeam(Connections.Count % 2 == 1);
        playerData.SetUsername($"Player {Connections.Count}");
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        Player player = CreateGamePlayer(roomPlayer);
        Players.Add(player);
        return player.gameObject;
    }

    private Player CreateGamePlayer(GameObject roomPlayer)
    {
        var connection = roomPlayer.GetComponent<Connection>();
        var playerData = connection.GetComponent<PlayerData>();
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
        if (conn.identity == null) { return; }
        var connection = conn.identity.GetComponent<Connection>();
        Connections.Remove(connection);
    }

    public override void OnRoomStopHost()
    {
        Connections.Clear();
    }

    #endregion

    #region Client

    #endregion

}

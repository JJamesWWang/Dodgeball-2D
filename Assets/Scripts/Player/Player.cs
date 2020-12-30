using UnityEngine;
using Mirror;
using System;

// Properties: ConnectionNetId, Username, IsLeftTeam, IsRightTeam
// Events: ServerPlayerHit, ClientPlayerSpawned
// Methods: [Server] SetConnection, [Server] EnableInput, [Server] DisableInput, 
public class Player : NetworkBehaviour
{
    [SyncVar]
    private uint connectionNetId;
    private PlayerData data;
    private PlayerCommands commands;
    private PlayerMovement movement;
    private PlayerArm arm;
    private Room room;

    public uint ConnectionNetId { get { return connectionNetId; } }
    public string Username { get { return data.Username; } }
    public bool IsLeftTeam { get { return data.IsLeftTeam; } }
    public bool IsRightTeam { get { return data.IsRightTeam; } }

    public static event Action<Player> ServerPlayerHit;
    public static event Action<Player> ClientPlayerSpawned;

    private void Awake()
    {
        commands = GetComponent<PlayerCommands>();
        movement = GetComponent<PlayerMovement>();
        arm = GetComponent<PlayerArm>();
    }

    #region Server

    public override void OnStartServer()
    {
        room = (Room)NetworkManager.singleton;
        room.AddPlayer(this);
    }

    public override void OnStopServer()
    {
        room.RemovePlayer(this);
    }

    [Server]
    public void SetConnection(Connection connection)
    {
        connectionNetId = connection.netId;
        data = connection.PlayerData;
    }

    [Server]
    public void EnableInput()
    {
        commands.TargetSetInputEnabled(true);
    }

    [Server]
    public void DisableInput()
    {
        commands.TargetSetInputEnabled(false);
        movement.StopMovement();
        arm.StopThrow();
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D other)
    {
        ServerPlayerHit?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        room = (Room)NetworkManager.singleton;
        data = FindPlayerData();
        if (!NetworkServer.active)
            room.AddPlayer(this);
        ClientPlayerSpawned?.Invoke(this);
    }

    [Client]
    private PlayerData FindPlayerData()
    {
        foreach (Connection connection in room.Connections)
            if (connection.netId == ConnectionNetId)
                return connection.PlayerData;
        return null;
    }

    public override void OnStopClient()
    {
        if (!NetworkServer.active)
            room.RemovePlayer(this);
    }

    #endregion Client

}

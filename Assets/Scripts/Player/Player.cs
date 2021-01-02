using UnityEngine;
using Mirror;
using System;

// Properties: ConnectionNetId, Team, IsLeftTeam, IsRightTeam, IsSpectator, Username, 
// Events: ServerPlayerHit, ServerPlayerDisconnected, ClientPlayerSpawned
// Methods: [Server] SetConnection, [Server] EnableInput, [Server] DisableInput, 
public class Player : NetworkBehaviour
{
    // Temporarily serialized for debugging purposes
    [SyncVar]
    [SerializeField] private uint connectionNetId;
    private PlayerData data;
    [SyncVar]
    [SerializeField] private Color color;
    private PlayerCommands commands;
    private PlayerMovement movement;
    private PlayerArm arm;
    private SpriteColorer spriteColorer;
    private Room room;

    public uint ConnectionNetId { get { return connectionNetId; } }
    public Team Team { get { return data.Team; } }
    public bool IsLeftTeam { get { return data.IsLeftTeam; } }
    public bool IsRightTeam { get { return data.IsRightTeam; } }
    public bool IsSpectator { get { return data.IsSpectator; } }
    public string Username { get { return data.Username; } }
    public Color Color { get { return spriteColorer.Color; } }


    public static event Action<Player> ServerPlayerHit;
    public static event Action<Player> ServerPlayerDisconnected;
    public static event Action<Player> ClientPlayerSpawned;

    private void Awake()
    {
        commands = GetComponent<PlayerCommands>();
        movement = GetComponent<PlayerMovement>();
        arm = GetComponent<PlayerArm>();
        spriteColorer = GetComponent<SpriteColorer>();
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
        ServerPlayerDisconnected?.Invoke(this);
    }

    [Server]
    public void SetConnection(Connection connection)
    {
        connectionNetId = connection.netId;
        data = connection.PlayerData;
        SetColor();
    }

    [Server]
    private void SetColor()
    {
        if (IsLeftTeam)
            spriteColorer.SetColor(GameState.Instance.LeftTeamColor);
        else if (IsRightTeam)
            spriteColorer.SetColor(GameState.Instance.RightTeamColor);
    }

    [Server]
    public void EnableInput()
    {
        commands.SetServerInputEnabled(true);
    }

    [Server]
    public void DisableInput()
    {
        commands.SetServerInputEnabled(false);
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
        Debug.LogError("No Player found in FindPlayerData().");
        return null;
    }

    public override void OnStopClient()
    {
        if (!NetworkServer.active)
            room.RemovePlayer(this);
    }

    #endregion Client

}

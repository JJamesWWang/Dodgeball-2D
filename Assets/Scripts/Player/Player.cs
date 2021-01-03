using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;

// Properties: ConnectionNetId, Ready, Team, IsLeftTeam, IsRightTeam, IsSpectator, Username, 
// Events: ServerPlayerHit, ServerPlayerDisconnected, ClientLocalPlayerStarted, ClientPlayerSpawned
// Methods: [Server] SetConnection, [Server] SetColor, [Server] SetOnField, [Server] SetInputEnabled
public class Player : NetworkBehaviour
{
    // Temporarily serialized for debugging purposes
    [SyncVar]
    [SerializeField] private uint connectionNetId;
    private PlayerData data;
    [SyncVar]
    [SerializeField] private TeamsConfig teamsConfig;
    [SerializeField] private Color color;
    private PlayerCommands commands;
    private PlayerMovement movement;
    private PlayerArm arm;
    [SyncVar(hook = nameof(HandleSpriteVisibleUpdated))]
    [SerializeField] private bool isSpriteVisible;

    [SyncVar(hook = nameof(HandleCollisionEnabledUpdated))]
    [SerializeField] private bool isCollisionEnabled;

    private CapsuleCollider2D bodyCollider;
    [SerializeField] private List<SpriteRenderer> sprites;
    private SpriteColorer spriteColorer;
    private Room room;

    public uint ConnectionNetId { get { return connectionNetId; } }
    /// <summary> Indicates whether the Player is connected and ready to play </summary>
    public bool Ready { get; private set; }
    public bool IsLocalPlayer { get { return isLocalPlayer; } }
    public Team Team { get { return data.Team; } }
    public bool IsLeftTeam { get { return data.IsLeftTeam; } }
    public bool IsRightTeam { get { return data.IsRightTeam; } }
    public bool IsSpectator { get { return data.IsSpectator; } }
    public string Username { get { return data.Username; } }
    public Color Color { get { return spriteColorer.Color; } }
    public bool IsOnField { get { return sprites[0].enabled && bodyCollider.enabled; } }


    public static event Action<Player> ServerPlayerHit;
    public static event Action<Player> ServerPlayerDisconnected;
    public static event Action<Player> ClientLocalPlayerStarted;
    public static event Action<Player> ClientPlayerSpawned;

    private void Awake()
    {
        commands = GetComponent<PlayerCommands>();
        movement = GetComponent<PlayerMovement>();
        arm = GetComponent<PlayerArm>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        spriteColorer = GetComponent<SpriteColorer>();
    }

    #region Server

    public override void OnStartServer()
    {
        room = (Room)NetworkManager.singleton;
        room.AddPlayer(this);
        SetOnField(false);
        SetInputEnabled(false);
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
            spriteColorer.SetColor(teamsConfig.LeftTeamColor);
        else if (IsRightTeam)
            spriteColorer.SetColor(teamsConfig.RightTeamColor);
    }

    /// <summary> Is the player physically present on the field? (toggles visibility & hit detection) </summary>
    [Server]
    public void SetOnField(bool isOn)
    {
        SetSpriteVisible(isOn);
        SetCollisionEnabled(isOn);
    }

    [Server]
    private void SetSpriteVisible(bool show)
    {
        foreach (var sprite in sprites)
            sprite.enabled = show;
        isSpriteVisible = show;
    }

    [Server]
    private void SetCollisionEnabled(bool enabled)
    {
        bodyCollider.enabled = enabled;
        isCollisionEnabled = enabled;
    }

    [Server]
    public void SetInputEnabled(bool enabled)
    {
        if (enabled)
        {
            commands.SetServerInputEnabled(true);
            return;
        }
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
        EventLogger.LogError("No Player found in FindPlayerData().");
        return null;
    }

    public override void OnStartLocalPlayer()
    {
        Ready = true;
        ClientLocalPlayerStarted?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!NetworkServer.active)
            room.RemovePlayer(this);
    }

    private void HandleSpriteVisibleUpdated(bool _oldVisibility, bool newVisibility)
    {
        foreach (var sprite in sprites)
            sprite.enabled = newVisibility;
    }

    private void HandleCollisionEnabledUpdated(bool _oldEnabled, bool newEnabled)
    {
        bodyCollider.enabled = newEnabled;
    }


    #endregion Client

}

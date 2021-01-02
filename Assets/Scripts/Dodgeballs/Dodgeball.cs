using UnityEngine;
using Mirror;
using System;

// Events: ServerDodgeballSpawned, ServerDodgeballDespawned
// Methods: [Server] SetPosition, [Server] SetVelocity, [Server] SetTeam
public class Dodgeball : NetworkBehaviour
{
    private Rigidbody2D body;
    [Tooltip("Number of bounces before Dodgeball disappears.")]
    [SerializeField] private int maxBounces = 3;
    private int timesBounced = 0;
    private SpriteRenderer visibilityChecker;
    private bool hasCrossedMiddleOnce;
    private Vector2 spawnPosition;
    private SpriteColorer spriteColorer;

    /// <summary> Invoked whenever a Dodgeball is instantiated </summary>
    public static event Action<Dodgeball> ServerDodgeballSpawned;
    /// <summary> Invoked only when a Dodgeball despawns internally (in this script) </summary>
    public static event Action<Dodgeball> ServerDodgeballDespawned;


    #region Server

    [ServerCallback]
    private void Awake()
    {
        visibilityChecker = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        spriteColorer = GetComponent<SpriteColorer>();
        spawnPosition = transform.position;
        ServerDodgeballSpawned?.Invoke(this);
    }

    [Server]
    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    [Server]
    public void SetVelocity(Vector2 velocity)
    {
        body.velocity = velocity;
    }

    [Server]
    public void SetTeam(Team team)
    {
        SetColor(team);
    }

    [Server]
    private void SetColor(Team team)
    {
        spriteColorer.SetTeamColor(team);
    }

    [ServerCallback]
    private void Update()
    {
        if (!visibilityChecker.isVisible || HasCrossedMiddleTwice())
            DestroySelf();
        CheckCrossedMiddleOnce();
    }

    [Server]
    private bool HasCrossedMiddleTwice()
    {
        float startSide = Mathf.Sign(spawnPosition.x);
        float currentSide = Mathf.Sign(transform.position.x);
        float movingDirection = Mathf.Sign(body.velocity.x);
        return hasCrossedMiddleOnce && startSide == currentSide && startSide == movingDirection;
    }

    private void CheckCrossedMiddleOnce()
    {
        if (hasCrossedMiddleOnce) { return; }
        float startSide = Mathf.Sign(spawnPosition.x);
        float currentSide = Mathf.Sign(transform.position.x);
        if (startSide != currentSide)
            hasCrossedMiddleOnce = true;
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
        ServerDodgeballDespawned?.Invoke(this);
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    [Server]
    private void HandleCollision(Collision2D collision)
    {
        var collidedObject = collision.collider.gameObject;
        if (IsPlayerCollision(collidedObject) || IsLastBounce())
            DestroySelf();
        timesBounced += 1;
    }

    [Server]
    private bool IsPlayerCollision(GameObject collidedObject)
    {
        return collidedObject.layer == LayerMask.NameToLayer("Player");
    }

    [Server]
    private bool IsLastBounce()
    {
        return timesBounced >= maxBounces;
    }

    #endregion
}

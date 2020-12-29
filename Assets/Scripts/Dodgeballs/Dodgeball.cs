using UnityEngine;
using Mirror;
using System;

public class Dodgeball : NetworkBehaviour
{
    [Tooltip("Number of bounces before Dodgeball disappears.")]
    [SerializeField] private int maxBounces = 3;
    private int timesBounced = 0;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private static Dodgeball dodgeballPrefab;

    public static event Action<Dodgeball> ServerDodgeballSpawned;
    public static event Action<Dodgeball> ServerDodgeballDespawned;


    #region Server

    [ServerCallback]
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        ServerDodgeballSpawned?.Invoke(this);
    }

    [ServerCallback]
    private void Update()
    {
        if (!spriteRenderer.isVisible)
        {
            NetworkServer.Destroy(gameObject);
            ServerDodgeballDespawned?.Invoke(this);
        }
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
        if (collidedObject.layer == LayerMask.NameToLayer("Player") ||
            timesBounced >= maxBounces)
        {
            NetworkServer.Destroy(gameObject);
            ServerDodgeballDespawned?.Invoke(this);
        }
        timesBounced += 1;
    }

    [Server]
    public void SetVelocity(Vector2 velocity)
    {
        body.velocity = velocity;
    }

    #endregion
}

﻿using UnityEngine;
using Mirror;
using System;

// Events: ServerDodgeballSpawned, ServerDodgeballDespawned
// Methods: [Server] SetVelocity
public class Dodgeball : NetworkBehaviour
{
    [Tooltip("Number of bounces before Dodgeball disappears.")]
    private Rigidbody2D body;
    [SerializeField] private int maxBounces = 3;
    private int timesBounced = 0;
    private SpriteRenderer spriteRenderer;

    /// <summary> Invoked whenever a Dodgeball is instantiated </summary>
    public static event Action<Dodgeball> ServerDodgeballSpawned;
    /// <summary> Invoked only when a Dodgeball despawns internally (in this script) </summary>
    public static event Action<Dodgeball> ServerDodgeballDespawned;


    #region Server

    [ServerCallback]
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        ServerDodgeballSpawned?.Invoke(this);
    }

    [Server]
    public void SetVelocity(Vector2 velocity)
    {
        body.velocity = velocity;
    }

    [ServerCallback]
    private void Update()
    {
        if (!spriteRenderer.isVisible)
            DestroySelf();
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
        if (CollidedWithPlayer(collidedObject) || BouncedMaxTimes())
            DestroySelf();
        timesBounced += 1;
    }

    [Server]
    private bool CollidedWithPlayer(GameObject collidedObject)
    {
        return collidedObject.layer == LayerMask.NameToLayer("Player");
    }

    [Server]
    private bool BouncedMaxTimes()
    {
        return timesBounced >= maxBounces;
    }

    #endregion
}

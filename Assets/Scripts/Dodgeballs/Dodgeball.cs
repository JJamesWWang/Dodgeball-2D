using UnityEngine;
using Mirror;
using System;

public class Dodgeball : NetworkBehaviour
{
    [Tooltip("Number of bounces before Dodgeball disappears.")]
    [SerializeField] private int maxBounces = 3;
    private int timesBounced = 0;

    public static event Action<Player> ServerPlayerHit;

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    [Server]
    private void HandleCollision(Collision2D collision)
    {
        var collidedObject = collision.collider.gameObject;
        if (collidedObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = collidedObject.GetComponent<Player>();
            ServerPlayerHit?.Invoke(player);
            Destroy(gameObject);
        }

        if (timesBounced >= maxBounces)
            Destroy(gameObject);
        timesBounced += 1;
    }

    [Server]
    public void SetVelocity(Vector2 velocity)
    {
        GetComponent<Rigidbody2D>().velocity = velocity;    // Using GetComponent<> because Start might not have been run yet
    }
}

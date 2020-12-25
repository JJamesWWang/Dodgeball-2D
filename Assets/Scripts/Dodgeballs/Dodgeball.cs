using UnityEngine;
using Mirror;
using System;

public class Dodgeball : NetworkBehaviour
{
    [Tooltip("Number of bounces before Dodgeball disappears.")]
    [SerializeField] private int maxBounces = 3;
    private int timesBounced = 0;
    [SerializeField] private static Dodgeball dodgeballPrefab;

    public static event Action<Dodgeball> ServerDodgeballSpawned;
    public static event Action<Dodgeball> ServerDodgeballDespawned;
    public static event Action<Player> ServerPlayerHit;


    #region Server

    [Server]
    public static Dodgeball Spawn(Dodgeball prefab, Vector3 position, Quaternion rotation)
    {
        Dodgeball dodgeball = Instantiate(prefab, position, rotation);
        ServerDodgeballSpawned?.Invoke(dodgeball);
        return dodgeball;
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
        Dodgeball dodgeball = collision.otherCollider.gameObject.GetComponent<Dodgeball>();
        if (collidedObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = collidedObject.GetComponent<Player>();
            ServerPlayerHit?.Invoke(player);
            NetworkServer.Destroy(gameObject);
            ServerDodgeballDespawned?.Invoke(dodgeball);
        }

        if (timesBounced >= maxBounces)
        {
            NetworkServer.Destroy(gameObject);
            ServerDodgeballDespawned?.Invoke(dodgeball);
        }
        timesBounced += 1;
    }

    [Server]
    public void SetVelocity(Vector2 velocity)
    {
        GetComponent<Rigidbody2D>().velocity = velocity;    // Using GetComponent<> because Start might not have been run yet
    }

    #endregion
}

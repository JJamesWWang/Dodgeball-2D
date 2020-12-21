using UnityEngine;
using Mirror;

public class Dodgeball : NetworkBehaviour
{
    [SerializeField] private int maxBounces = 3;
    [SerializeField] private float launchSpeed = 300f;
    private int timesBounced = 0;

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collidedObject = collision.collider.gameObject;
        if (collidedObject.layer == LayerMask.NameToLayer("Player"))
        {
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

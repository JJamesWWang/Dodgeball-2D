using UnityEngine;
using Mirror;

// Properties: ThrowPowerFrequency, ThrowPowerPeriod, TimeTo100Percent
// Methods: CmdStartThrow, CmdReleaseThrow, [Server] StopThrow
public class PlayerArm : NetworkBehaviour
{
    private Player player;
    [SerializeField] private Dodgeball dodgeballPrefab = null;
    private bool hasThrowStarted = false;
    private float throwStartTime = 0f;
    [SerializeField] private float minimumThrowSpeed = 100f;
    [SerializeField] private float maximumThrowSpeed = 500f;
    [Tooltip("1 over this value is how many seconds it takes to go from 0% power to 0% power again.")]
    [SerializeField] private float throwPowerFrequency = 2f;
    [Tooltip("Distance to throw from player's origin to avoid an immediate collision.")]
    [SerializeField] private float offsetDistance = 24f;
    [SerializeField] private float throwCooldown = 0.5f;
    private float cooldownTimer = 0f;

    public float ThrowPowerFrequency { get { return throwPowerFrequency; } }
    public float ThrowPowerPeriod { get { return 1 / throwPowerFrequency; } }
    public float TimeTo100Percent { get { return ThrowPowerPeriod / 2f; } }

    [ServerCallback]
    private void Start()
    {
        player = GetComponent<Player>();
    }

    [ServerCallback]
    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    #region Server

    [Command]
    public void CmdStartThrow()
    {
        if (hasThrowStarted) { return; }
        throwStartTime = Time.time;
        hasThrowStarted = true;
    }

    [Command]
    public void CmdReleaseThrow(Vector2 throwAtPoint)
    {
        if (!hasThrowStarted) { return; }
        if (IsOnCooldown() || IsThrowingAtSelf(throwAtPoint))
        {
            hasThrowStarted = false;
            return;
        }
        ThrowDodgeball(throwAtPoint);
        cooldownTimer = throwCooldown;
        hasThrowStarted = false;
    }

    [Server]
    private bool IsThrowingAtSelf(Vector2 throwAtPoint)
    {
        Vector2 throwFromOrigin = transform.position;
        float distanceSquared = (throwAtPoint - throwFromOrigin).sqrMagnitude;
        return distanceSquared < offsetDistance * offsetDistance;
    }

    [Server]
    private bool IsOnCooldown()
    {
        return cooldownTimer > 0f;
    }

    [Server]
    private void ThrowDodgeball(Vector2 throwAtPoint)
    {
        Vector2 throwFromPoint = CalculateThrowFromPoint(throwAtPoint);
        Vector2 throwVelocity = CalculateThrowVelocity(throwFromPoint, throwAtPoint);
        // Spawn position is important for Dodgeball; set it at the Player and then offset it.
        Dodgeball dodgeball = Instantiate(dodgeballPrefab, transform.position, Quaternion.identity);
        dodgeball.SetPosition(throwFromPoint);
        dodgeball.SetVelocity(throwVelocity);
        dodgeball.SetTeam(player.Team);
        NetworkServer.Spawn(dodgeball.gameObject);
    }

    [Server]
    private Vector2 CalculateThrowFromPoint(Vector2 throwAtPoint)
    {
        Vector2 throwFromOrigin = transform.position;
        Vector2 throwDirection = (throwAtPoint - throwFromOrigin).normalized;
        return throwFromOrigin + (throwDirection * offsetDistance);
    }

    [Server]
    private Vector2 CalculateThrowVelocity(Vector2 throwFromPoint, Vector2 throwAtPoint)
    {
        float throwPowerPercentage = CalculateThrowPowerPercentage();
        float throwSpeed = Mathf.Lerp(minimumThrowSpeed, maximumThrowSpeed, throwPowerPercentage);
        Vector2 throwDirection = (throwAtPoint - throwFromPoint).normalized;
        return throwSpeed * throwDirection;
    }

    [Server]
    private float CalculateThrowPowerPercentage()
    {
        float actualTimePassed = CalculateActualTimePassed();
        if (actualTimePassed <= TimeTo100Percent)
            return actualTimePassed / TimeTo100Percent;
        return (TimeTo100Percent - actualTimePassed % TimeTo100Percent) / TimeTo100Percent;
    }

    [Server]
    private float CalculateActualTimePassed()
    {
        float throwReleaseTime = Time.time;
        float rawTimePassed = throwReleaseTime - throwStartTime;
        return rawTimePassed % ThrowPowerPeriod;
    }

    [Server]
    public void StopThrow()
    {
        hasThrowStarted = false;
    }

    #endregion
}

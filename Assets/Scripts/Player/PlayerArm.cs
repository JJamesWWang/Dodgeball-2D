using UnityEngine;
using Mirror;

// Properties: ThrowPowerFrequency, ThrowPowerPeriod, TimeTo100Percent
// Methods: CmdStartThrow, CmdReleaseThrow, [Server] StopThrow
public class PlayerArm : NetworkBehaviour
{
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
    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    #region Server

    [Command]
    public void CmdStartThrow()
    {
        if (hasThrowStarted || cooldownTimer > 0f) { return; }
        throwStartTime = Time.time;
        hasThrowStarted = true;
    }

    [Command]
    public void CmdReleaseThrow(Vector2 throwAtPoint)
    {
        if (!hasThrowStarted) { return; }
        ThrowDodgeball(throwAtPoint);
        cooldownTimer = throwCooldown;
        hasThrowStarted = false;
    }

    [Server]
    private void ThrowDodgeball(Vector2 throwAtPoint)
    {
        Vector2 throwFromPoint = CalculateThrowFromPoint(throwAtPoint);
        Dodgeball dodgeball = Instantiate(dodgeballPrefab, throwFromPoint, Quaternion.identity);
        Vector2 throwVelocity = CalculateThrowVelocity(throwFromPoint, throwAtPoint);
        dodgeball.SetVelocity(throwVelocity);
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

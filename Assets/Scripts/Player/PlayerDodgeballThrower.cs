using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class PlayerDodgeballThrower : NetworkBehaviour
{
    private bool hasThrowStarted = false;
    private float throwStartTime = 0f;
    [SerializeField] private float minimumThrowSpeed = 100f;
    [SerializeField] private float maximumThrowSpeed = 500f;
    [Tooltip("1 over this value is how many seconds it takes to go from 0% power to 0% power again.")]
    [SerializeField] private float throwPowerFrequency = 2f;
    [Tooltip("Distance to throw from player's origin to avoid an immediate collision.")]
    [SerializeField] private float offsetDistance = 24f;
    [SerializeField] private float throwCooldown = 0.5f;
    private float timeSinceLastThrow = 0f;

    public float ThrowPowerFrequency { get { return throwPowerFrequency; } }
    public float ThrowPowerPeriod { get { return 1 / throwPowerFrequency; } }
    public float TimeTo100Percent { get { return ThrowPowerPeriod / 2f; } }

    [SerializeField] private Dodgeball dodgeballPrefab = null;

    [ServerCallback]
    private void Update()
    {
        timeSinceLastThrow -= Time.deltaTime;
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
        if (timeSinceLastThrow > 0f)
        {
            hasThrowStarted = false;
            return;
        }

        ThrowDodgeball(throwAtPoint);
        timeSinceLastThrow = throwCooldown;
        hasThrowStarted = false;
    }

    [Server]
    private void ThrowDodgeball(Vector2 throwAtPoint)
    {
        float throwPowerPercentage = CalculatePowerPercentage();
        float throwSpeed = Mathf.Lerp(minimumThrowSpeed, maximumThrowSpeed, throwPowerPercentage);
        Vector2 throwFromPoint = transform.position;
        Vector2 throwDirection = (throwAtPoint - throwFromPoint).normalized;
        Vector2 throwVelocity = throwSpeed * throwDirection;
        throwFromPoint += (throwDirection * offsetDistance);
        Dodgeball dodgeball = Dodgeball.Spawn(dodgeballPrefab, throwFromPoint, Quaternion.identity);
        dodgeball.SetVelocity(throwVelocity);
        NetworkServer.Spawn(dodgeball.gameObject);
    }

    [Server]
    private float CalculatePowerPercentage()
    {
        float throwReleaseTime = Time.time;
        float rawTimePassed = throwReleaseTime - throwStartTime;
        float actualTimePassed = rawTimePassed % ThrowPowerPeriod;

        float throwPower;
        if (actualTimePassed <= TimeTo100Percent)
            throwPower = actualTimePassed / TimeTo100Percent;
        else
            throwPower = (TimeTo100Percent - (actualTimePassed % TimeTo100Percent)) / TimeTo100Percent;

        float throwPowerPercentage = ((int)(throwPower * 100f)) / 100f;
        return throwPowerPercentage;
    }

    #endregion
}

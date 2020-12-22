using UnityEngine;
using Mirror;

public class PlayerArm : NetworkBehaviour
{
    private bool hasThrowStarted = false;
    private float throwStartTime = 0f;
    [Tooltip("Velocity of throw at 0% power.")]
    [SerializeField] private float minimumThrowPower = 10f;
    [Tooltip("Velocity of throw at 100% power.")]
    [SerializeField] private float maximumThrowPower = 30f;
    [Tooltip("1 over this value is how many seconds it takes to go from 0% power to 0% power again.")]
    [SerializeField] private float throwPowerFrequency = 5f;
    public float ThrowPowerFrequency { get { return throwPowerFrequency; } }
    public float ThrowPowerPeriod { get { return 1 / throwPowerFrequency; } }
    public float TimeTo100Percent { get { return ThrowPowerPeriod / 2f; } }

    [Command]
    public void CmdStartThrow()
    {
        if (hasThrowStarted) { return; }
        throwStartTime = Time.time;
        hasThrowStarted = true;
    }

    [Command]
    public void CmdReleaseThrow()
    {
        int throwPowerPercentage = CalculatePowerPercentage();
        Debug.Log(throwPowerPercentage);
        hasThrowStarted = false;
    }

    private int CalculatePowerPercentage()
    {
        float throwReleaseTime = Time.time;
        float rawTimePassed = throwReleaseTime - throwStartTime;
        float actualTimePassed = rawTimePassed % ThrowPowerPeriod;

        float throwPower;
        if (actualTimePassed <= TimeTo100Percent)
            throwPower = actualTimePassed / TimeTo100Percent;
        else
            throwPower = (TimeTo100Percent - (actualTimePassed % TimeTo100Percent)) / TimeTo100Percent;

        int throwPowerPercentage = (int)(throwPower * 100f);
        return throwPowerPercentage;
    }
}

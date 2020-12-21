using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private bool isLeftTeam;
    [SerializeField] private Dodgeball dodgeballPrefab;

    public bool IsLeftTeam { get { return isLeftTeam; } }

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (NetworkServer.active) { return; }
        DodgeballNetworkManager networkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        networkManager.Players.Add(this);
    }
    #endregion

    #region Server

    [Server]
    public void SetIsLeftTeam(bool value)
    {
        isLeftTeam = value;
    }

    [ContextMenu("Launch Ball")]
    private void TestLaunchBall()
    {
        Dodgeball dodgeball = Instantiate(dodgeballPrefab, Vector3.zero, Quaternion.identity);
        dodgeball.SetVelocity(new Vector2(0f, 100f));
        NetworkServer.Spawn(dodgeball.gameObject);
    }

    #endregion

}

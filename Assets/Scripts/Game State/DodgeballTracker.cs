using UnityEngine;
using Mirror;
using System.Collections.Generic;

// Methods: [Server] DespawnDodgeballs
public class DodgeballTracker : NetworkBehaviour
{
    [SerializeField] private List<Dodgeball> dodgeballs = new List<Dodgeball>();

    #region Server

    [Server]
    public void DespawnDodgeballs()
    {
        foreach (Dodgeball dodgeball in dodgeballs)
            NetworkServer.Destroy(dodgeball.gameObject);
        dodgeballs.Clear();
    }

    public override void OnStartServer()
    {
        SubscribeEvents();
    }

    public override void OnStopServer()
    {
        UnsubscribeEvents();
    }

    [Server]
    private void SubscribeEvents()
    {
        Dodgeball.ServerDodgeballSpawned += HandleDodgeballSpawned;
        Dodgeball.ServerDodgeballDespawned += HandleDodgeballDespawned;
    }

    [Server]
    private void HandleDodgeballSpawned(Dodgeball dodgeball)
    {
        dodgeballs.Add(dodgeball);
    }

    [Server]
    private void HandleDodgeballDespawned(Dodgeball dodgeball)
    {
        dodgeballs.Remove(dodgeball);
    }

    [Server]
    private void UnsubscribeEvents()
    {
        Dodgeball.ServerDodgeballSpawned -= HandleDodgeballSpawned;
        Dodgeball.ServerDodgeballDespawned -= HandleDodgeballDespawned;
    }

    #endregion
}

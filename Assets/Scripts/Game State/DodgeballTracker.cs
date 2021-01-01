using UnityEngine;
using Mirror;
using System.Collections.Generic;

// Methods: [Server] DespawnDodgeballs
public class DodgeballTracker : NetworkBehaviour
{
    [SerializeField] private List<Dodgeball> dodgeballs = new List<Dodgeball>();

    private void OnEnable()
    {
        SubscribeEvents();
    }
    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    #region Server

    [Server]
    public void DespawnDodgeballs()
    {
        foreach (Dodgeball dodgeball in dodgeballs)
            NetworkServer.Destroy(dodgeball.gameObject);
        dodgeballs.Clear();
    }

    [ServerCallback]
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

    [ServerCallback]
    private void UnsubscribeEvents()
    {
        Dodgeball.ServerDodgeballSpawned -= HandleDodgeballSpawned;
        Dodgeball.ServerDodgeballDespawned -= HandleDodgeballDespawned;
    }

    #endregion
}

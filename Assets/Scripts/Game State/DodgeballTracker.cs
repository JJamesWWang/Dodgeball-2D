using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class DodgeballTracker : NetworkBehaviour
{
    [SerializeField] private List<Dodgeball> dodgeballs = new List<Dodgeball>();
    public override void OnStartServer()
    {
        Dodgeball.ServerDodgeballSpawned += HandleDodgeballSpawned;
        Dodgeball.ServerDodgeballDespawned += HandleDodgeballDespawned;
    }

    public override void OnStopServer()
    {
        Dodgeball.ServerDodgeballSpawned -= HandleDodgeballSpawned;
        Dodgeball.ServerDodgeballDespawned -= HandleDodgeballDespawned;
    }

    private void HandleDodgeballSpawned(Dodgeball dodgeball)
    {
        dodgeballs.Add(dodgeball);
    }

    private void HandleDodgeballDespawned(Dodgeball dodgeball)
    {
        dodgeballs.Remove(dodgeball);
    }

    [Server]
    public void DespawnDodgeballs()
    {
        foreach (Dodgeball dodgeball in dodgeballs)
        {
            NetworkServer.Destroy(dodgeball.gameObject);
        }
        dodgeballs.Clear();
    }
}

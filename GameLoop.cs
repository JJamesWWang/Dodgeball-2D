using UnityEngine;
using Mirror;

public class GameLoop : NetworkBehaviour
{
    [SerializeField] private int scoreToWin = 11;
    private int leftTeamScore;
    private int rightTeamScore;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Dodgeball.ServerPlayerHit += HandlePlayerHit;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Dodgeball.ServerPlayerHit -= HandlePlayerHit;
    }

    [Server]
    private void HandlePlayerHit(Player player)
    {
        
    }
}

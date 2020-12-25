using UnityEngine;
using Mirror;

public class GameState : MonoBehaviour
{
    private MatchTracker matchTracker;

    private void Start()
    {
        matchTracker = GetComponent<MatchTracker>();
    }

    #region Server

    [Server]
    public void StartGame()
    {
        matchTracker.StartMatch();
    }

    #endregion
}

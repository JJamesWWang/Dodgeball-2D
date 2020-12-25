using Mirror;
using System;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private PlayerConnection _connection;
    public PlayerConnection Connection { get { return _connection; } }

    #region Server

    [Server]
    public void SetConnection(PlayerConnection connection)
    {
        _connection = connection;
    }

    #endregion
}

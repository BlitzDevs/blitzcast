using UnityEngine;
using Mirror;
using System.Collections.Generic;

/// <summary>
/// The NetworkGameManager is the NetworkManager which has references
/// to current connections.
/// </summary>
public class NetworkGameManager : NetworkManager
{

    public int numConnected = 0;
    public GameManager gameManager;
    private Player playerA;
    private Player playerB;


    public override void OnServerConnect(NetworkConnection conn)
    {
        numConnected++;
        if (numConnected == 1) {
            // playerA = TargetGetInfo(conn);
        } else if (numConnected == 2) {
            // playerB = TargetGetInfo(conn);
            gameManager.RpcStartGame();
        }
        Debug.Log("PlayerConnected: " + numConnected);
    }
   

}

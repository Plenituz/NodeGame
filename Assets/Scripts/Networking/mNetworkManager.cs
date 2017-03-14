using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class mNetworkManager : NetworkManager {
    private bool spawnedBTM = false;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        PlayerNetwork.instance.RpcUpdateCam();
        Debug.Log("player connected");

        if (!spawnedBTM)
        {
            spawnedBTM = true;
            GameObject btm = Instantiate(Resources.Load<GameObject>("BulletTimeManager"));
            NetworkServer.SpawnWithClientAuthority(btm, conn);
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        spawnedBTM = false;
    }
}

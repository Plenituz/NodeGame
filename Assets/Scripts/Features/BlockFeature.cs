using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BlockFeature : Blocker {
    private const float blockTime = 1f;
    private const float blockCoolDown = 3f;

    private float lastBlock = -10f;

    [SyncVar] public bool isBlocking = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (Time.time - lastBlock > blockCoolDown)
                {
                    lastBlock = Time.time;
                    CmdSetBlock(true);
                }
            }
            if (isBlocking && Time.time - lastBlock > blockTime)
                CmdSetBlock(false);
        }

        if (isBlocking && sr.color != Color.yellow)
            sr.color = Color.yellow;
        if (!isBlocking && sr.color != Color.red && isLocalPlayer)
            sr.color = Color.red;
        if (!isBlocking && sr.color != Color.white && !isLocalPlayer)
            sr.color = Color.white;
    }

    [Command]
    void CmdSetBlock(bool b)
    {
        isBlocking = b;
    }

    public override void GetHit(GameObject hitter)
    {
        PlayerNetwork.instance.CmdDestroyObject(hitter);
        PlayerNetwork.instance.CmdDestroyObject(gameObject);
    }

    public override bool IsBlocking(){
#if true
        return isBlocking;
#else
        return true;
#endif
    }


	public override void Block(GameObject attacker){
		//Debug.Log ("blocked attack");
	}
}

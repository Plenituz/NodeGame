using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class BulletTimeManager : NetworkBehaviour {
    int playerLayer;
    public float lastStop = -10f;
    const float coolDown = 0.5f;
    
	void Start () {
        playerLayer = LayerMask.GetMask("Player");
	}
	
	void Update () {
        if (hasAuthority && PlayerNetwork.instance != null)
        {
            bool doSlow = false;
            for(int i = 0; i < Bullet.bullets.Count; i++)
            {
                if(Bullet.bullets[i] == null)
                {
                    Bullet.bullets.RemoveAt(i);
                    continue;
                }
                Transform b = Bullet.bullets[i].transform;
                doSlow = EventailRaycast(b);
                if (doSlow)
                    break;
            }

            if (doSlow && PlayerNetwork.instance.mTimeScale == 1f)
            {
                PlayerNetwork.instance.CmdSetTimeScale(0.3f);
                lastStop = Time.time;
            }

            if (Time.time - lastStop > coolDown)
            {
                if (!doSlow && PlayerNetwork.instance.mTimeScale != 1f)
                {
                    PlayerNetwork.instance.CmdSetTimeScale(1f);
                    lastStop = Time.time;
                }
            }
           
        }
	}

    public bool EventailRaycast(Transform b)
    {
        int max = 7;
        float minAngle = 40f;
        for(int i = 0; i < max; i++)
        {
            Vector2 dir = Quaternion.Euler(0f, 0f, -Mathf.Lerp(minAngle, 180f - minAngle, i/(float)max)) * b.up;
            RaycastHit2D hit = Physics2D.Raycast(b.position, dir, 6f, playerLayer);
            Debug.DrawLine(b.position, b.position + (Vector3)dir.normalized*6f, hit.point != Vector2.zero ? Color.red : Color.white);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }
}

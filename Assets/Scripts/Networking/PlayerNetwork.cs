using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour {
	private GameObject bulletPrefab;
	private float startTime;

	public static PlayerNetwork instance;
    public static float timeScale = 1f;

	[SyncVar(hook ="OnTimeScaleChange")]//hooks c'est de la merde, je le fait a la main dans le update
    public float mTimeScale = 1f;
	[SyncVar (hook = "OnRotChange")]
	public float rot = 0f;

	public override void OnStartLocalPlayer()
	{
		instance = this;
		//Debug.Log ("start local player " + isLocalPlayer);
		GetComponent<SpriteRenderer> ().color = Color.red;
	}

	void Start(){
		if (isLocalPlayer)
			Feature.EnableAll (gameObject);
		else {
			Feature.DestroyAll (gameObject);
			//We still need the behaviour so the server has the same 
		}
	}

	void Update(){
		if (!isLocalPlayer)
			return;

        if (Time.timeScale != timeScale)
            Time.timeScale = timeScale;

        if (Time.time - startTime > 1 / 9f) {//sync rotation, since I dont use the rigidbody's rotation
			startTime = Time.time;
			if(rot != transform.rotation.eulerAngles.z)
				CmdSetRot (transform.rotation.eulerAngles.z);
		}
	}

    void OnTimeScaleChange(float t)
    {
        mTimeScale = t;
        timeScale = t;
    }

    public void OnRotChange(float newRot){
		rot = newRot;
		transform.rotation = Quaternion.Euler (0f, 0f, rot);
	}

	[Command]
	public void CmdSetTimeScale(float t){
		this.mTimeScale = t;
        Debug.Log("time scale set");
	}

	[Command]
	public void CmdSetRot(float newRot){
		this.rot = newRot;
	}

	[Command]//runs on server
	public void CmdFire(GameObject proprio, Vector2 fireFrom, Vector2 fireTo){
		if (bulletPrefab == null)
			bulletPrefab = Resources.Load<GameObject> ("Bullet");

		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		//bullet.transform.position = fireFrom;
		Bullet bullSc = bullet.GetComponent<Bullet> ();
		bullSc.from = fireFrom;
		bullSc.to = fireTo;
		bullSc.proprio = proprio;
		Destroy (bullet, 5f);

		NetworkServer.Spawn (bullet);
	}

	[Command]
	public void CmdDestroyObject(GameObject g){
		NetworkServer.Destroy (g);
	}

    [Command]
    public void CmdSetProprio(GameObject b, GameObject pr)
    {
        b.GetComponent<Bullet>().proprio = pr;
    }

    [Command]
    public void CmdAskAuthority(NetworkIdentity id)
    {
        id.AssignClientAuthority(connectionToClient);
        Debug.Log("set authority to " + id);
    }

    [ClientRpc]
    public void RpcUpdateCam()
    {
        Camera.main.GetComponent<CameraControl>().UpdatePlayerList();
    }
}

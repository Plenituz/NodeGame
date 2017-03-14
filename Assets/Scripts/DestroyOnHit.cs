using UnityEngine;
using System.Collections;

public class DestroyOnHit : MonoBehaviour, Destroyable {

	public void Hit(GameObject hitter){
		PlayerNetwork.instance.CmdDestroyObject (gameObject);
		PlayerNetwork.instance.CmdDestroyObject (hitter);
	}
}

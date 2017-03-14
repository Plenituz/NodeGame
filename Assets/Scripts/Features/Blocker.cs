using System;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Blocker : NetworkBehaviour, Destroyable {
	public void Hit(GameObject hitter){
		if (IsBlocking ()) {
			Bouncable bo = hitter.GetComponent<Bouncable> ();
			if (bo != null)
				bo.Bounce (gameObject);
			Block (hitter);
		}else
			GetHit (hitter);
	}

	public abstract void GetHit(GameObject hitter);
	public abstract bool IsBlocking();
	public abstract void Block(GameObject attacker);
}



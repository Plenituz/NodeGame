using System;
using UnityEngine;

public class Wall : Blocker
{
    public bool bounce = false;

	public override void GetHit(GameObject hitter){
		PlayerNetwork.instance.CmdDestroyObject (hitter);
	}


	public override bool IsBlocking(){
		return bounce;
	}


	public override void Block(GameObject attacker){
		//Debug.Log ("blocked attack");
	}
}



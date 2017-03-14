using System;
using UnityEngine;

/*
 * Declare this GameObject as bouncable, meaning when it hits something that is blocking it's going to bounce off of it
 * 
 */
public interface Bouncable : Destroyable
{
	void Bounce(GameObject bounceOnto);	
}



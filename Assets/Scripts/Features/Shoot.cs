using UnityEngine;
using System.Collections;

public class Shoot : Feature {
    private GunPoint gunPoint;

    void Start()
    {
        GameObject gp = Instantiate(Resources.Load<GameObject>("GunPoint"));
        gp.transform.parent = transform;
        gp.transform.localPosition = Vector2.zero;

        gunPoint = gp.GetComponent<GunPoint>();
    }
	
	void Update () {
		if (Input.GetMouseButtonDown (0))
			Fire ();
	}

	void Fire(){
		PlayerNetwork.instance.CmdFire (gameObject, gunPoint.pointPos, (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}
}

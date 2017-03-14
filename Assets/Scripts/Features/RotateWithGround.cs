using UnityEngine;
using System.Collections;

public class RotateWithGround : Feature {
	float raySpread = 0.2f;

	//memory buffer so you don't calculate it every frame
	Transform rayStart;
	Quaternion rotAvg;
	int layerMask;
	Vector3 right;
	Vector3 left;

	void Start () {
        rayStart = new GameObject("Raycast").transform;
        rayStart.parent = transform;
        rayStart.localPosition = new Vector2(0f, -0.0653f);

		layerMask = LayerMask.GetMask (new string[]{ "Wall", "Ground" });
		rotAvg = Quaternion.Euler (0f, 0f, -90f);
		right = (Vector3)Vector2.right * raySpread;
		left = (Vector3)Vector2.left * raySpread;
	}
	
	void Update () {
		//3 raycast : left mid right, get the average
		RaycastHit2D rayHit1 = Physics2D.Raycast (rayStart.position, Vector2.down, 0.2f, layerMask);
		RaycastHit2D rayHit2 = Physics2D.Raycast (rayStart.position + right, Vector2.down, 0.2f, layerMask);
		RaycastHit2D rayHit3 = Physics2D.Raycast (rayStart.position + left , Vector2.down, 0.2f, layerMask);

		Vector2 avgNormal = (rayHit1.normal + rayHit2.normal + rayHit3.normal) / 3f;
        Quaternion rot = Utils.LookAt2D(rotAvg * avgNormal);
        

        float z = Mathf.LerpAngle(transform.rotation.eulerAngles.z, rot.eulerAngles.z, Time.deltaTime * 20f);
        
        transform.rotation = Quaternion.Euler(0f, 0f, z);

#if true
        Debug.DrawLine (rayHit1.point, rayHit1.point + rayHit1.normal, Color.magenta);
		Debug.DrawLine (rayHit2.point, rayHit2.point + rayHit2.normal, Color.magenta);
		Debug.DrawLine (rayHit3.point, rayHit3.point + rayHit3.normal, Color.magenta);
		#endif
	}
}

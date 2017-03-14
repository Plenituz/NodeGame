using UnityEngine;
using System.Collections;

public class WallJump : Feature{
	private const float wallJumpSpeed = 10f;

	Rigidbody2D rg;
	BoxCollider2D rightWall;
	BoxCollider2D leftWall;
	CircleCollider2D groundCheck;

	//int groundMask;
	int wallMask;
	int wallDir;

	bool onWall = false;
	bool onGround = false;
	bool jumpAxis = false;

	void Start () {
		//groundMask = LayerMask.GetMask (new string[]{ "Ground" });
		wallMask = LayerMask.GetMask (new string[]{ "Wall" });

		rg = GetComponent<Rigidbody2D> ();
		groundCheck = GetComponent<CircleCollider2D> ();

		rightWall = transform.FindChild ("WallCheckRight").GetComponent<BoxCollider2D> ();
		leftWall = transform.FindChild ("WallCheckLeft").GetComponent<BoxCollider2D> ();
	}
	
	void Update () {
		jumpAxis = Input.GetAxisRaw ("Vertical") == 1 || Input.GetAxisRaw ("Jump") == 1;
		wallDir = rightWall.IsTouchingLayers (wallMask) ? 1 : leftWall.IsTouchingLayers (wallMask) ? -1 : 0;

		onWall = wallDir != 0;
		onGround = groundCheck.IsTouchingLayers ();

	}

	void FixedUpdate(){
		if (onWall && !onGround && jumpAxis) {
			//wall jump
			//wall direction a la place de -1
			Vector2 wallJump = new Vector2(-wallDir * wallJumpSpeed * 0.75f, wallJumpSpeed); 
			rg.velocity = wallJump;
			//Debug.DrawLine (transform.position, transform.position + (Vector3)wallJump, Color.gray);
		}
	}
}

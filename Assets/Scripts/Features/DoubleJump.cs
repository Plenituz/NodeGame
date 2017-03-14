using UnityEngine;
using System.Collections;

public class DoubleJump : Feature {
	private const float doubleJumpSpeed = 14f;

	Rigidbody2D rg;
	CircleCollider2D groundCheck;
	BoxCollider2D rightWall;
	BoxCollider2D leftWall;
	//int groundMask;
	int wallMask;
	int wallDir;

	bool onGround = false;
	bool jumpAxis = false;
	bool jumpDown = false;
	bool canDoubleJump = true;
	bool hasWallJump = false;
	bool onWall = false;


	void Start () {
		//groundMask = LayerMask.GetMask (new string[]{ "Ground" });
		rg = GetComponent<Rigidbody2D> ();
		groundCheck = GetComponent<CircleCollider2D> ();
		hasWallJump = GetComponent<WallJump> () != null;

		if (hasWallJump) {
			rightWall = transform.FindChild ("WallCheckRight").GetComponent<BoxCollider2D> ();
			leftWall = transform.FindChild ("WallCheckLeft").GetComponent<BoxCollider2D> ();
			wallMask = LayerMask.GetMask (new string[]{ "Wall" });
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (hasWallJump) {
			wallDir = rightWall.IsTouchingLayers (wallMask) ? 1 : leftWall.IsTouchingLayers (wallMask) ? -1 : 0;
			onWall = wallDir != 0;
		}

		onGround = groundCheck.IsTouchingLayers ();

		if (onGround)
			canDoubleJump = true;

		bool newJumpAxis = Input.GetAxisRaw ("Vertical") == 1 || Input.GetAxisRaw ("Jump") == 1;
		if (newJumpAxis && !jumpAxis) {
			jumpDown = true;
		} else {
			jumpDown = false;
		}
		jumpAxis = newJumpAxis;
	}

	void FixedUpdate(){
		if (hasWallJump) {
			if (!onGround && canDoubleJump && jumpDown) {
				rg.velocity = new Vector2 (rg.velocity.x, doubleJumpSpeed);
				canDoubleJump = false;
			}
		} else {
			if (!onGround && !onWall && canDoubleJump && jumpDown) {
				rg.velocity = new Vector2 (rg.velocity.x, doubleJumpSpeed);
				canDoubleJump = false;
			}
		}
	}
}

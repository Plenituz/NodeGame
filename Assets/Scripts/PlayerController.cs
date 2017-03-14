using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Feature {
	//constants
	public const float speed = 16;
    public const float groundSpeed = 10f;
    public const float airSpeed = 3f;
    public const float jumpSpeed = 14f;
    public const float gravity = 3f;

	//unity shit
	Rigidbody2D rg;
	CircleCollider2D groundCheck;
	//int groundMask;

	//movement vars
	float horizontalAxis = 0f;
	bool jumpAxis = false;
	bool onGround = false;

    public LockingManager lockManager;

	void Start(){
		//groundMask = LayerMask.GetMask (new string[]{ "Ground", "Player" });
		rg = GetComponent<Rigidbody2D> ();
		groundCheck = GetComponent<CircleCollider2D> ();

		rg.gravityScale = gravity;
		rg.freezeRotation = true;
		rg.useAutoMass = true;
        lockManager = new GameObject("Lock Manager").AddComponent<LockingManager>();
	}

    void OnDestroy()
    {
        if (lockManager != null)
            Destroy(lockManager);
    }

	void Update(){
		horizontalAxis = Input.GetAxis ("Horizontal");
		onGround = groundCheck.IsTouchingLayers ();
		jumpAxis = Input.GetAxisRaw ("Vertical") == 1 || Input.GetAxisRaw ("Jump") == 1;

		/*if (Input.GetKeyDown (KeyCode.A)) {
			if(PlayerNetwork.instance.mTimeScale != 0.3f)
				PlayerNetwork.instance.CmdSetTimeScale (0.3f);
		}
		if(Input.GetKeyDown(KeyCode.W)){
			if(PlayerNetwork.instance.mTimeScale != 1f) 
				PlayerNetwork.instance.CmdSetTimeScale (1f);
		}*/
	}
		
	void FixedUpdate () {
        float forceX = ((horizontalAxis * speed) - rg.velocity.x) * (onGround ? groundSpeed : airSpeed);
        Vector2 force = Utils.Forward2D(transform) * forceX;
		rg.AddForce(force); //move player
        //Debug.Log((horizontalAxis * speed) - rg.velocity.x);
		//Debug.DrawLine (transform.position, transform.position + (Vector3)force, Color.blue);
			
		Vector2 midVel = new Vector2 (
			                 rg.velocity.x, 
			                 (jumpAxis && onGround) ? jumpSpeed : rg.velocity.y);
		rg.velocity = midVel;
        Debug.DrawLine(transform.position, transform.position + (Vector3)rg.velocity);

        //Debug.DrawLine (transform.position, transform.position + (Vector3)midVel, Color.green);
        //Stop player if input.x is 0 (and grounded) and jump if input.y is 1
    }
}
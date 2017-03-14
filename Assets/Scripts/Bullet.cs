using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour, Bouncable {
    public static List<GameObject> bullets = new List<GameObject>();

	private const float speed = 10f;
	private Rigidbody2D rg;

	[SyncVar] public GameObject proprio;
	[SyncVar] public Vector2 from;
	[SyncVar] public Vector2 to;

	void Start(){
        bullets.Add(gameObject);
		transform.position = from;
		Vector2 direction = (to - from).normalized;

		rg = GetComponent<Rigidbody2D> ();
		rg.velocity = direction * speed;
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Rad2Deg*Mathf.Atan2(direction.y, direction.x)));
	}

	void Update(){
        //Debug.DrawLine (transform.position, transform.position + (Quaternion.Euler(0f, 0f, -90f) * transform.up), Color.yellow);
        //Debug.DrawLine (transform.position, transform.position + (Vector3)rg.velocity, Color.cyan);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)Utils.Forward2D(transform), Color.green);
    }

	public void Hit(GameObject hitter){
		PlayerNetwork.instance.CmdDestroyObject (gameObject);
		PlayerNetwork.instance.CmdDestroyObject (hitter);
    }

	public void Bounce(GameObject bounceOnto){
		//the actual bouncyness is done by the physicsmaterial2D on the bullet
		if (bounceOnto.CompareTag ("Player")) {
            //bounce manually
            //or put an actual collider to the player when he is blocking ?
            //bounceOnto.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            rg.velocity = new Vector2(-rg.velocity.x, rg.velocity.y);
            Debug.Log("player");
		}

		transform.rotation = Utils.LookAt2D (rg.velocity);
        rg.velocity = rg.velocity.normalized * speed;
        PlayerNetwork.instance.CmdSetProprio(gameObject,  bounceOnto);

	}

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject != proprio) {
            Destroyable destroyable = coll.collider.GetComponent<Destroyable>();
            if (destroyable != null)
                destroyable.Hit(gameObject);
         //   if (Time.time - startTime > 0.2f)
            //    Hit(gameObject);
        }
	}
}

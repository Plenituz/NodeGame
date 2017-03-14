using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    public Vector2 direction;
    public Vector2 startPos;
    public Action<GameObject> onHit;
    public GameObject shooter;

    private BoxCollider2D coll;

    public static Projectile Create(Vector2 direction, Vector2 startPos, Action<GameObject> onHit, GameObject shooter)
    {
        GameObject go = new GameObject("Projectile");
        Projectile pro = go.AddComponent<Projectile>();
        pro.direction = direction;
        pro.startPos = startPos;
        pro.onHit = onHit;
        pro.shooter = shooter;

        return pro;
    }

    void Start()
    {
        transform.position = startPos;
        transform.localScale = new Vector3(0.2532076f, 0.2532076f, 1f);
       // transform.LookAt(startPos + direction);

        Rigidbody2D rg = gameObject.AddComponent<Rigidbody2D>();
        rg.gravityScale = 0f;
        rg.velocity = direction;

        SpriteRenderer img = gameObject.AddComponent<SpriteRenderer>();
        img.sprite = Resources.Load<Sprite>("deathNumber");

        coll = gameObject.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;
        Destroy(gameObject, 7f);
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        Transform p = c.gameObject.transform;
        while(p.parent != null)
        {
            if (p.gameObject == shooter)
                return;
            p = p.parent;
        }
        if (p.gameObject == shooter)
            return;

        if(onHit != null)
            onHit(c.gameObject);
        Destroy(gameObject);
    }

}

using UnityEngine;
using System.Collections;

public class GunPoint : MonoBehaviour {
    private Vector2 mouseWorldPos;
    public Vector2 pointPos;
    int mask;

    void Start()
    {
        mask = LayerMask.GetMask(new string[] { "GunPoint" });
    }
	
	void Update () {
        mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseWorldPos - (Vector2)transform.position, 2f, mask);
        pointPos = hit.point;

        Debug.DrawRay(transform.position, ((Vector2)transform.position + mouseWorldPos - (Vector2)transform.position).normalized);
        Debug.DrawLine(pointPos, pointPos + Vector2.right, Color.blue);
    }
}

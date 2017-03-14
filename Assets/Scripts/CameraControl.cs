using UnityEngine;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {
    private GameObject[] players;
    private Camera cam;
    private float padding = 6f;

	void Start () {
        cam = GetComponent<Camera>();
        UpdatePlayerList();
	}

    public void UpdatePlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
	
	void Update ()
    {
        if (players.Length == 0)
            return;

        Vector3 avg = Vector2.zero;
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i] == null)
            {
                UpdatePlayerList();
                return;
            }
            avg += players[i].transform.position;
        }
        avg /= players.Length;
        avg.z = -10f;
        transform.position = avg;

        float dist = Vector2.Distance((Vector2)transform.position, (Vector2)players[0].transform.position);
        cam.orthographicSize = dist + padding;
	}
}

using UnityEngine;
using System.Collections;

public class LockingManager : MonoBehaviour {
    public GameObject target;

    private GameObject lockImg;

	void Start ()
    {
        lockImg = Instantiate(Resources.Load<GameObject>("Target"));
        lockImg.SetActive(false);
        lockImg.transform.SetParent(transform, false);
	}
	
	void Update ()
    {
        MoveTarget();
        CheckControl();
	}

    void CheckControl()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            //left alt change target to the one closest to the mouse
            Damageable[] list = FindObjectsOfType<Damageable>();
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Damageable closest = list[0];
            float closestDist = Vector2.Distance(pos, list[0].transform.position);
            for (int i = 0; i < list.Length; i++)
            {
                if(list[i].GetComponent<PlayerController>() == null)
                {
                    float distance = Vector2.Distance(pos, list[i].transform.position);
                    if (distance < closestDist)
                    {
                        closestDist = distance;
                        closest = list[i];
                    }
                }
            }
            if(closest.GetComponent<PlayerController>() == null)
            {
                target = closest.gameObject;
            }
        }
    }

    void MoveTarget()
    {
        if (target != null)
        {
            if (!lockImg.activeSelf)
                lockImg.SetActive(true);
            lockImg.transform.position = target.transform.position;
        }
        else
        {
            if (lockImg.activeSelf)
                lockImg.SetActive(false);
        }
    }
}

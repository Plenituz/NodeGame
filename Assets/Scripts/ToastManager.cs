using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Collections;

public class ToastManager : MonoBehaviour {
    public GameObject canvas;

    List<KeyValuePair<float, string>> pendingToasts = new List<KeyValuePair<float, string>>();
    GameObject currentToast;

    internal void RequestFollowToast(float time, string text)
    {
        if(pendingToasts.Count < 5)
            pendingToasts.Add(new KeyValuePair<float, string>(time, text));
    }

    void Update()
    {
        if(currentToast == null && pendingToasts.Count != 0)
        {
            GameObject g = GameObject.Instantiate(Resources.Load<GameObject>("Toast")) as GameObject;
            Text t = g.GetComponent<Text>();
            g.transform.SetParent(canvas.transform, false);
            KeyValuePair<float, string> pair = pendingToasts[0];
            pendingToasts.RemoveAt(0);

            t.text = pair.Value;
            Toast toast = g.GetComponent<Toast>();
            toast.time = pair.Key;
            toast.followMouse = true;
            currentToast = g;
            Debug.Log("new toast" + currentToast);
        }
    }
}

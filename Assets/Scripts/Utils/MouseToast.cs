using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MouseToast {
    static GameObject canvas;
    static ToastManager toastManager;

    static void InitCanvas()
    {
        canvas = GameObject.Instantiate(Resources.Load<GameObject>("ToastCanvas")) as GameObject;
        toastManager = canvas.GetComponent<ToastManager>();
        toastManager.canvas = canvas;
    }

	public static void MakeToastFollowMouse(float time, string text)
    {
        if (canvas == null)
            InitCanvas();
        toastManager.RequestFollowToast(time, text);
    }

    public static void MakeToastFixed(float time, string text, Vector2 pos)
    {
        if(canvas == null)
            InitCanvas();
        GameObject g = GameObject.Instantiate(Resources.Load<GameObject>("Toast")) as GameObject;
        Text t = g.GetComponent<Text>();
        g.transform.SetParent(canvas.transform, false);

        t.text = text;
        g.transform.position = pos;
        g.GetComponent<Toast>().time = time;
    }
}

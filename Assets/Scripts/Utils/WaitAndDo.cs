using System;
using System.Collections;
using UnityEngine;

public class WaitAndDo : MonoBehaviour
{
    public Action<object> act;
    public object arg;

    void Start()
    {
        StartCoroutine(WaitAndDoStuff(act, arg));
    }

    IEnumerator WaitAndDoStuff(Action<object> act, object arg)
    {
        yield return new WaitForEndOfFrame();
        if (act != null)
            act(arg);
        Destroy(gameObject);
    }
}


using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Toast : MonoBehaviour {
    Image bg;
    Text t;
    Vector2 posOffset;

    public float time;
    public bool followMouse = false;

	void Start()
    {
        GameObject g = new GameObject("Toast BG");
        bg = g.AddComponent<Image>();
        bg.transform.SetParent(transform.parent, false);
        bg.transform.position = transform.position;
        t = GetComponent<Text>();
        RectTransform rt = t.GetComponent<RectTransform>();
        float width = LayoutUtility.GetPreferredWidth(rt);
        float height = LayoutUtility.GetPreferredHeight(rt);
        posOffset = new Vector2(width + 20f, height + 20f)/2;

        bg.GetComponent<RectTransform>().sizeDelta = posOffset*2;
        bg.transform.SetAsFirstSibling();
        bg.raycastTarget = false;

        t.CrossFadeAlpha(0f, 0f, true);
        t.CrossFadeAlpha(1f, 0.5f, true);
        bg.CrossFadeAlpha(0f, 0f, true);
        bg.CrossFadeAlpha(1f, 0.5f, true);
        StartCoroutine(DoomDay());   
    }

    void Update()
    {
        if (followMouse)
        {
            Vector2 pos = (Vector2)Input.mousePosition - posOffset;
            transform.position = pos;
            bg.transform.position = pos;
        }
    }

    IEnumerator DoomDay()
    {
        yield return new WaitForSecondsRealtime(time - 0.5f);
        t.CrossFadeAlpha(0f, 0.5f, true);
        bg.CrossFadeAlpha(0f, 0.5f, true);
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(bg.gameObject);
        Destroy(gameObject);
    }
}

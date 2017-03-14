using UnityEngine;
using System.Collections.Generic;
using VisualEditor.BackEnd;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Events;
using System.Text.RegularExpressions;

public class UIListSpell : MonoBehaviour {
    public GameObject row;

    List<GameObject> rows = new List<GameObject>();
    private RectTransform rectTranform;
    private ScrollRect scroll;

	void Start () {
        scroll = GetComponentInParent<ScrollRect>();
        scroll.verticalNormalizedPosition = 1f;
        rectTranform = GetComponent<RectTransform>();

        UpdateList();
	}

    void UpdateList()
    {
        while(rows.Count != 0)
        {
            Destroy(rows[0]);
            rows.RemoveAt(0);
        }

        string[] list = Spell.GetList();
        Regex regex = new Regex(@".*(\\|/)");
        for (int i = 0; i < list.Length; i++)
        {
            CreateRow(regex.Replace(list[i], "").Replace(".SpellDic", ""));
        }
        rectTranform.sizeDelta = new Vector2(rectTranform.sizeDelta.x, 70f + 50f * rows.Count);
    }

    void CreateRow(string name)
    {
        GameObject go = Instantiate(row) as GameObject;
        go.transform.SetParent(transform, false);
        go.name = name;
        go.transform.FindChild("Name").GetComponent<Text>().text = name;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, -50f + rows.Count * -50f);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 50f);
        rows.Add(go);
    }
	
    public void Delete(string del)
    {
        //TODO confirmation message
        File.Delete(Application.dataPath + "/Spells/" + del + ".SpellDic");
        File.Delete(Application.dataPath + "/Spells/" + del + ".Spell");
        UpdateList();
    }

    public void Open(string op)
    {
        GameObject g = new GameObject("InputManager - " + op);
        InputManager inp = g.AddComponent<InputManager>();
        inp.saveName = op;
        StartCoroutine(WaitLoad(inp, op));
    }

    IEnumerator WaitLoad(InputManager inp, string name)
    {
        yield return new WaitForEndOfFrame();
        inp.Load(name);
      // GetComponentInParent<Canvas>().gameObject.SetActive(false);
        Destroy(gameObject.GetComponentInParent<Canvas>().gameObject);
    }

    public void New()
    {
        GameObject g = new GameObject("InputManager - new");
        g.AddComponent<InputManager>();
        Destroy(gameObject.GetComponentInParent<Canvas>().gameObject);
        //GetComponentInParent<Canvas>().gameObject.SetActive(false);
        //MsgBox.Make("swag ou quoi mon gars\nbah ouais jsuis comme swag\n\nezefsdf\nfdsfsdfsdf", new string[] { "j", "j", "jj" }, new MsgBox.OnButtonClick[] { (MsgBox msgbox, object arg) => Debug.Log(arg), (MsgBox msgbox, object arg) => Debug.Log(arg), (MsgBox msgbox, object arg) => { Debug.Log(arg); msgbox.Close(); } }, new object[] { "pd", "swag", "prout" });
    }
}

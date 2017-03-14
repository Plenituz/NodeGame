using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using VisualEditor.Visuals;
using UnityEngine.UI;

public class SplatMenu : MonoBehaviour {
    public delegate void OnSelected(string selected);

    public NodeVisual host;
    public string[] choices;//this has to be set when creating the object
    public bool open = false;
    public OnSelected onSelected;
    public Vector2 itemSize;
    public float radiusMult;
    public Color color = Color.yellow;
    public bool selectOnStart = true;

    private List<GameObject> childs = new List<GameObject>();
    private bool canMove = true;
    public GameObject selected;

   // RectTransform rectTransform;

    public static SplatMenu Create(NodeVisual parent, string[] list, OnSelected onselected, Vector2 itemSize, float radius, bool selectOnStart)
    {
        GameObject r = new GameObject("SplatMenu");
        r.transform.SetParent(parent.transform, false);
        SplatMenu splat = r.AddComponent<SplatMenu>();
        splat.choices = list;
        splat.onSelected = onselected;
        splat.host = parent;
        splat.itemSize = itemSize;
        splat.radiusMult = radius;
        splat.selectOnStart = selectOnStart;
        return splat;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        for(int i = 0; i < childs.Count; i++)
        {
            SplatItem splat = childs[i].GetComponent<SplatItem>();
            splat.color = color;
            splat.GetComponent<Image>().color = color;
        }
    }

	void Start () {
        //rectTransform = gameObject.AddComponent<RectTransform>();
        if(gameObject.GetComponent<RectTransform>() == null)
            gameObject.AddComponent<RectTransform>();
        UpdateFromList();
        if(selectOnStart)
            Select(childs[0].GetComponent<SplatItem>());
	}

    public void UpdateList(string[] nList)
    {
        string selectedString = "";
        if (selected != null)
            selectedString = selected.GetComponent<SplatItem>().GetText();

        choices = nList;
        UpdateFromList();

        var q = from l in childs
                where l.GetComponent<SplatItem>().GetText().Equals(selectedString)
                select l;
        GameObject s = q.FirstOrDefault();
        if (s != null)
            Select(s.GetComponent<SplatItem>());
        else
            Select(childs[0].GetComponent<SplatItem>());
    }

    public SplatItem GetItemForText(string t)
    {
        var q = from c in childs
                where c.GetComponent<SplatItem>().GetText().Equals(t)
                select c.GetComponent<SplatItem>();
        return q.FirstOrDefault();
    }

    void UpdateFromList()
    {
        for(int i = 0; i < choices.Length; i++)
        {
            if (i < childs.Count)
            {
                //update child
                //if the selected if updated, call OnSelected
                SplatItem comp = childs[i].GetComponent<SplatItem>();
                comp.active = true;
                comp.SetText(choices[i]);
            }
            else
            {
                //create child
                SplatItem child = CreateChild();
                child.active = true;
                float radius = i < 6 ? 1 : i < 18 ? 2 : 3;
                                   
                float max = i < 6 ?
                          6 :
                          i < 18 ?
                          12 :
                          24;
                float lerp = i < 6 ?
                            i / max :
                            i < 18 ?
                            (i - 6) / max :
                            (i - 18) / max;
                /* float lerp = i < 6 ?
                             i / 6f :
                             i < 18 ?
                             (i - 6) / 12f :
                             (i - 18) / 24f;*/
                float pos = Mathf.Lerp(0f, Mathf.PI * 2, lerp);
                radius *= radiusMult;

                child.dockPos = new Vector2(Mathf.Cos(pos) * radius, Mathf.Sin(pos) * radius);
                child.SetText(choices[i]);
                if (i != 0)
                    child.gameObject.SetActive(false);
            }
        }
        for (int i = choices.Length; i < childs.Count; i++)
        {
            //if the selected is deactivated, call set the selected to 0 and call OnSelected
            //deactivate the ones in too many
            SplatItem splat = childs[i].GetComponent<SplatItem>();
            splat.active = false;
            splat.SetText("inactive");
            childs[i].SetActive(false);
            if (selected == childs[i])
            {
                selected = childs[0];
                if (onSelected != null)
                    onSelected(childs[0].GetComponent<SplatItem>().GetText());
            }
        }
    }

    public void Open()
    {
        if (!canMove || choices.Length <= 1)
            return;
        if (!open)
        {
            StartCoroutine(OpenCor());
        }
    }

    IEnumerator OpenCor()
    {
        open = true;
        canMove = false;
        SplatItem furthest = GetFurthestActiveChild();
        for (int i = 0; i < childs.Count; i++)
        {
            SplatItem splat = childs[i].GetComponent<SplatItem>();
            if (splat.active)
            {
                if(splat == furthest)//this is the furthest child, wait till it arrived to continue
                {
                    yield return splat.GoToDockCor();
                }
                else
                {
                    StartCoroutine(splat.GoToDockCor());
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        canMove = true;
    }

    public void Select(SplatItem item)
    {
        Select(item, false);
    }

    public void Select(SplatItem item, bool discreet)
    {
        if (!open)
        {
            if (selected != null)
                selected.gameObject.SetActive(false);
            if (childs[0].gameObject.activeSelf)
                childs[0].SetActive(false);
            selected = item.gameObject;
            if (!discreet && onSelected != null)
                onSelected(item.GetText());
            item.gameObject.SetActive(true);
            return;
        }
        if (!canMove)
            return;
        if (open)
        {
            StartCoroutine(SelectCor(item));
        }
    }

    IEnumerator SelectCor(SplatItem item)
    {
        open = false;
        canMove = false;
        for (int i = 0; i < childs.Count; i++)
        {
            SplatItem splat = childs[i].GetComponent<SplatItem>();
            if (splat.active)
            {
                if (i == childs.Count - 1)
                    yield return splat.GoToRestCor(splat != item);
                else
                    StartCoroutine(splat.GoToRestCor(splat != item));
            }
        }
        selected = item.gameObject;
        if (onSelected != null)
            onSelected(item.GetText());
        canMove = true;
    }

    private SplatItem GetFurthestActiveChild()
    {
        for(int i = childs.Count - 1; i > 0; i--)
        {
            SplatItem sp = childs[i].GetComponent<SplatItem>();
            if (sp.active)
                return sp;
        }
        return null;
    }

    SplatItem CreateChild()
    {
        GameObject child = new GameObject("child_" + childs.Count);
        child.transform.SetParent(transform, false);
        SplatItem splat = child.AddComponent<SplatItem>();
        splat.host = this;
        splat.size = itemSize;
        splat.color = color;
        childs.Add(child);
        return splat;
    }
}

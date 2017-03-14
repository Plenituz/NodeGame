using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using VisualEditor.BackEnd;
using System.Reflection;
using VisualEditor.Visuals;

public class TabMenuList : MonoBehaviour {
    public delegate void OnNodeCreated(NodeVisual node);
    public OnNodeCreated onNodeCreated;

    public const float ITEM_HEIGHT = 30f;
    public const int MODE_STRING = 1;
    public const int MODE_INPUT = 2;
    public const int MODE_OUTPUT = 3;

    private static GameObject inputFieldPrefab = null;

    public Canvas parent;//these has to be set on creation of this object
    public Canvas spawnNodeIn;
    public Vector2 pos;
    public InputManager host;

    public int mode = 1;
    public Type[] filterTypes;

    private Dictionary<string, Type> list = new Dictionary<string, Type>();
    private bool reloadDisplay = false;
    private RectTransform rectTransform;
    private List<GameObject> createdChild = new List<GameObject>();
    private InputField inputField;
    public ScrollRect scrollView;

    private ListItem selected = null;

	void Start () {
        rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent.transform, false);
        rectTransform.position = pos;

        GameObject scroll = Instantiate(Resources.Load<GameObject>("ScrollView"));
        scrollView = scroll.GetComponent<ScrollRect>();
        scrollView.transform.SetParent(rectTransform, false);
        scrollView.scrollSensitivity = 15;
        scrollView.movementType = ScrollRect.MovementType.Clamped;
        scrollView.horizontal = false;

        RectTransform scrollt = scroll.GetComponent<RectTransform>();
        float height = Screen.height - (Screen.height - pos.y);
        scrollt.position = pos - new Vector2(0f, height / 2f - 20f);
        scrollt.sizeDelta = new Vector2(160f, height);


        if (inputFieldPrefab == null)
        {
            inputFieldPrefab = Resources.Load<GameObject>("InputField");
        }
        inputField = (Instantiate(inputFieldPrefab) as GameObject).GetComponent<InputField>();
        inputField.transform.SetParent(transform, false);
        inputField.transform.position = pos + new Vector2(0f, ITEM_HEIGHT + 5f);
        inputField.onValueChanged.AddListener(OnValueChange);
        inputField.onEndEdit.AddListener(OnEndEdit);
        ((Text)inputField.placeholder).text = "search";
        inputField.textComponent.alignment = TextAnchor.MiddleCenter;
        FocusInputField();

        ReloadDisplay();
        if (createdChild.Count != 0)
            Select(createdChild[0].GetComponent<ListItem>());
	}

    void OnValueChange(string val)
    {
        switch (mode)
        {
            case MODE_STRING:
                UpdateList(TabMenu.GetListForString(val));
                break;
            case MODE_INPUT:
                UpdateList(TabMenu.GetListForInputTypesAndString(filterTypes, val));
                break;
            case MODE_OUTPUT:
                UpdateList(TabMenu.GetListForOutputTypesAndString(filterTypes, val));
                break;
        }
    }

    void OnEndEdit(string val)
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            if (selected.gameObject.activeSelf)
                CreateSelectedNodeAndHide();//this finish the menu only when you click enter
            else
                gameObject.SetActive(false);
        }
    }

    public void CreateSelectedNodeAndHide()
    {
        Type nodeType;
        try
        {
            nodeType = list[selected.GetText()];
        }
        catch (KeyNotFoundException)
        {
            return;
        }
        MethodInfo getVisual = nodeType.GetMethod("GetAssociatedVisualClass");
        var node = Activator.CreateInstance(nodeType);
        Type visualType = (Type)getVisual.Invoke(node, null);

        NodeVisual.Create(visualType, host,
            new Vector2(UnityEngine.Input.mousePosition.x / Screen.width, UnityEngine.Input.mousePosition.y / Screen.height),
            spawnNodeIn.gameObject, true);

        gameObject.SetActive(false);
    }

    void Update () {
        if (reloadDisplay)
        {
            reloadDisplay = false;
            ReloadDisplay();
        }
        
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
        if(UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            //arrow up
            if(selected.index != 0)
            {
                Select(createdChild[selected.index - 1].GetComponent<ListItem>());
            }
        }
        if(UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            //arrow down
            if (selected.index + 1 != createdChild.Count && createdChild[selected.index + 1].activeSelf) 
            {
                Select(createdChild[selected.index + 1].GetComponent<ListItem>());
            }
        }
	}

    void ReloadDisplay()
    {
        int i = 0;
        foreach(KeyValuePair<string, Type> pair in list)
        {
            if(createdChild.Count > i)
            {
                //there is a child created, just change it's content
                createdChild[i].SetActive(true);
                createdChild[i].GetComponent<ListItem>().SetText(pair.Key);
            }
            else
            {
                //there is no child, create one and add it to the list
                GameObject child = CreateChild();
                RectTransform rt = child.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(11f, -(ITEM_HEIGHT + 5f) * i - 20f);
                //child.transform.position = /*pos + */new Vector2(0f, -(ITEM_HEIGHT+5f) * i);

                child.GetComponent<ListItem>().SetText(pair.Key);
            }
            i++;
        }
        for(; i < createdChild.Count; i++)
        {
            createdChild[i].SetActive(false);
        }
        scrollView.content.sizeDelta = new Vector2(0f, (ITEM_HEIGHT + 5f) * list.Count + 20f);
        scrollView.content.anchoredPosition = Vector2.zero;
        if (selected != null && !selected.gameObject.activeSelf)
            Select(createdChild[0].GetComponent<ListItem>());
    }

    GameObject CreateChild()
    {
        GameObject child = new GameObject("child_" + createdChild.Count);
        child.transform.SetParent(scrollView.content, false);
        
        ListItem it = child.AddComponent<ListItem>();
        it.host = this;
        it.index = createdChild.Count;

        createdChild.Add(child);
        return child;
    }

    public void Show(Vector2 atPos)
    {
        gameObject.SetActive(true);
        ClearInput();
        FocusInputField();
        transform.position = atPos;

        float height = Screen.height - (Screen.height - atPos.y);
        RectTransform scrollt = scrollView.GetComponent<RectTransform>();
        scrollt.position = atPos - new Vector2(0f, height / 2f - 20f);
        scrollt.sizeDelta = new Vector2(160f, height);
    }

    public void Select(ListItem item)
    {
        if(selected != null)
            selected.SetSelected(false);
        selected = item;
        item.SetSelected(true);
    }

    public void UpdateList(Dictionary<string, Type> nList)
    {
        list = nList;
        reloadDisplay = true;
    }

    public void ClearInput()
    {
        inputField.text = "";
    }

    public void FocusInputField()
    {
        inputField.ActivateInputField();
        inputField.Select();
    }

    public void SetMode(Type[] filter, int mode)
    {
        filterTypes = filter;
        this.mode = mode;
    }
}

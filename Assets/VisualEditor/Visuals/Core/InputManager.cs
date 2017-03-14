using UnityEngine;
using System.Collections.Generic;
using VisualEditor.Visuals;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using VisualEditor.Visuals.Impl;
using System.Linq;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using VisualEditor.BackEnd.Impl;
using System.Text.RegularExpressions;
using CielaSpike;

public class InputManager : MonoBehaviour {
    public static bool proMode = true;
    public static VisualEditor.BackEnd.Node[] clipboard;
    public static Vector2[] clipboardPos;
    public static InputManager activeManager;

    public string saveName = "No Name Yet";
    public List<RectTransform> allNodes = new List<RectTransform>();
    public List<RectTransform> selectedNodes = new List<RectTransform>();
    public bool groupMode = false;
    public InputManager levelUpIfGroup;
    public GroupNode hostIfGroup;
    public bool hidden = false;
    public VisualEditor.BackEnd.Spell spell;
    internal GameObject editorUI;
    internal Canvas overUI;
    CanvasScaler scaler;

    private float targetScaleFactor = 1f;
    private Vector2 lastMousePos;
    private TabMenuList tabMenu = null;
    private GameObject UIGlobal;
    private RectTransform selectionSquare;
    private List<RectTransform> safeFromDeselect = new List<RectTransform>();
    private List<RectTransform> safeFromSelect = new List<RectTransform>();
    private SplatMenu splatRightClick;
    private bool firstSelectSplat = true;
    private bool forceOverrideSave = false;
    private bool autoSave = false;
    private Text autoSaveText;
    //shift click
    private Vector2 oldMousePos;
    private bool wasInShiftingLink = false;
    private InputVisual[] bufferInputs;
    private InputVisual oldTarget;

    public void OnClickSave()
    {
        Save();
    }

    public void OnClickBack()
    {
        hostIfGroup.UpdateInputs();
        hostIfGroup.UpdateOutputs();
        var q = from n in levelUpIfGroup.allNodes
                where n.GetComponent<NodeVisual>().node == hostIfGroup
                select n.GetComponent<NodeVisual>();
        q.FirstOrDefault().UpdateLabels();

        levelUpIfGroup.Show();
        activeManager = levelUpIfGroup;
        Hide();
    }

    public void Hide()
    {
        hidden = true;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        hidden = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void OnNameChange(string s)
    {
        if (groupMode)
        {
            hostIfGroup.groupName = s;
        }
        else
        {
            saveName = s;
        }
    }

    public void Save()
    {
        Debug.Log("save");
        InputManagerSerializer dick = new InputManagerSerializer();
        for(int i = 0; i < allNodes.Count; i++)
        {
            dick.Add(allNodes[i].GetComponent<NodeVisual>().node, new Position2(allNodes[i].position));
        }
        dick.PrepareForSerialization();
        if (forceOverrideSave)
        {
            DoSave(null, dick);
        }
        else
        {
            if (File.Exists(Application.dataPath + "/Spells/" + saveName + ".SpellDic"))
            {
                //ask player if he wants to override
                //if not return;
                MsgBox.Make("A spell with this name: \"" + saveName + "\" already exists !\nDo you want to overwrite it ?\n(the old one will be lost)",
                    new string[] { "JUST DO IT!", "Don't do it bro" },
                    new MsgBox.OnButtonClick[] { DoSave, DontDoSave },
                    new object[] { dick });
            }
            else
            {
                DoSave(null, dick);
            }
        }
    }

    void DoSave(MsgBox msg, object o)
    {
        FileStream file = null;
        try
        {
            Directory.CreateDirectory(Application.dataPath + "/Spells");

            InputManagerSerializer dick = (InputManagerSerializer)o;
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Create(Application.dataPath + "/Spells/" + saveName + ".SpellDic");
            Debug.Log(file.Name);
            bf.Serialize(file, dick);
        }
        catch(Exception e)
        {
            MsgBox.Make("Couln't save spell dictionnary (" + saveName + ") to disk because:\n" + e.Message);
        }
        finally
        {
            if (file != null)
                file.Close();
        }
        GetSpell().SaveSpellToDisk();
        forceOverrideSave = true;
        if (msg != null)
            msg.Close();
    }

    void DontDoSave(MsgBox msg, object o)
    {
        msg.Close();
    }

    public void Load(InputManagerSerializer dick)
    {
        forceOverrideSave = true;
        for (int i = 0; i < dick.nodes.Length; i++)
        {
            //Cree les node visual en settant tout bien les public var puis call CreateLinkVisualsForList() sur allnodes
            Type type = dick.nodes[i].GetAssociatedVisualClass();
            GameObject go = new GameObject(type.ToString());
            NodeVisual comp = (NodeVisual)go.AddComponent(type);
            comp.host = this;
          //  comp.caster = caster;
            comp.canvas = editorUI;
            comp.doStart = false;
            comp.node = dick.nodes[i];
            RectTransform rect = comp.gameObject.AddComponent<RectTransform>();
            rect.SetParent(editorUI.transform, false);
            allNodes.Add(rect);
        }
        StartCoroutine(LoadWait(allNodes, dick.positions));
        StartCoroutine(LinkVisualCreationWait(allNodes));
    }

    public void Load(string name)
    {
        InputManagerSerializer dick = null;
        if (File.Exists(Application.dataPath + "/Spells/" + name + ".SpellDic"))
        {
            FileStream file = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                file = File.Open(Application.dataPath + "/Spells/" + name + ".SpellDic", FileMode.Open);
                dick = (InputManagerSerializer)bf.Deserialize(file);
                Load(dick);
            }
            catch (Exception exx)
            {
                MsgBox.Make("Couldn't load spellDic (" + name + ") from disk because:\n" + exx.Message);
            }
            finally
            {
                if(file != null)
                    file.Close();
            }
        }
        else
        {
            Debug.LogError("spell at Spells/" + name + ".SpellDic" + " doesn't exist");
            return;
        }
        //dick is filled :3
    }

    IEnumerator LoadWait(List<RectTransform> comps, Position2[] poss)
    {
        SetNodesAnchorAtPos(Vector2.zero);
        yield return new WaitForEndOfFrame();
        for(int i = 0; i < comps.Count; i++)
        {
            NodeVisual comp = comps[i].GetComponent<NodeVisual>();
            Vector2 pos = poss[i].ToVector();

            comp.InitFromNode();
            comp.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    IEnumerator LoadWait(List<RectTransform> comps, Vector2[] poss)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < comps.Count; i++)
        {
            NodeVisual comp = comps[i].GetComponent<NodeVisual>();
            Vector2 pos = poss[i];

            comp.InitFromNode();
            comp.transform.position = pos;
        }
    }

    IEnumerator LinkVisualCreationWait(List<RectTransform> list)
    {
        yield return new WaitForEndOfFrame();
        CreateLinkVisualsForNodes(list);
    }

    void CreateLinkVisualsForNodes(List<RectTransform> nodes)
    {
        InputVisual[] inVis = FindObjectsOfType<InputVisual>();

        for(int i = 0; i < nodes.Count; i++)
        {
            NodeVisual nodeVis = nodes[i].GetComponent<NodeVisual>();
            for(int k = 0; k < nodeVis.node.outputs.Count; k++)
            {
                VisualEditor.BackEnd.Output o = nodeVis.node.outputs[k];
                for(int z = 0; z < o.destinations.Count; z++)
                {
                    GameObject link = new GameObject("output_" + k + "_link_" + z);
                    LinkVisual linkVis = link.AddComponent<LinkVisual>();
                    link.transform.SetParent(nodeVis.outputsVisuals[k].transform, false);

                    InputVisual desti = FindInputVisual(inVis, o.destinations[z]);
                    linkVis.Set(nodeVis.outputsVisuals[k].gameObject, desti.gameObject);
                    linkVis.outputThatCreatedMe = nodeVis.outputsVisuals[k];
                    linkVis.inputDestination = desti;
                    desti.outputConnectedTo = nodeVis.outputsVisuals[k];
                    nodeVis.outputsVisuals[k].links.Add(linkVis);
                }
            }
        }
    }

    private InputVisual FindInputVisual(InputVisual[] inVis, VisualEditor.BackEnd.Input input)
    {
        var q = from j in inVis
                where j.inputAttachedTo == input
                select j;
        return q.FirstOrDefault();
    }

    public void CopySelectedNodes()
    {
        Debug.Log("copy");
        //copy data
        VisualEditor.BackEnd.Node[] buffer = new VisualEditor.BackEnd.Node[selectedNodes.Count];
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        clipboardPos = new Vector2[buffer.Length];

        for(int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = selectedNodes[i].GetComponent<NodeVisual>().node;
            clipboardPos[i] = selectedNodes[i].position;
        }
        //duplicate nodes
        bf.Serialize(ms, buffer);
        ms.Position = 0;
        buffer = (VisualEditor.BackEnd.Node[])bf.Deserialize(ms);

        //disconnect inputs and outputs that are not linked to a node inside the selection
        for (int i = 0; i < buffer.Length; i++)
        {
            for(int k = 0; k < buffer[i].inputs.Count; k++)
            {
                if (buffer[i].inputs[k].outputConnectedTo != null && !buffer.Contains(buffer[i].inputs[k].outputConnectedTo.host))
                {
                    buffer[i].inputs[k].SetIncommingDataType(null);
                    buffer[i].inputs[k].outputConnectedTo = null;
                    buffer[i].inputs[k].host.PartialSetup();
                }
            }
            for(int k = 0; k < buffer[i].outputs.Count; k++)
            {
                VisualEditor.BackEnd.Input[] copy = buffer[i].outputs[k].destinations.ToArray();

                for (int z = 0; z < copy.Length; z++)
                {
                    if (!buffer.Contains(copy[z].host))
                    {
                        buffer[i].outputs[k].DisconnectFrom(copy[z]);
                    }
                }
            }
        }
        clipboard = buffer;
    }

    public void CopySelectedNodesForGrouping(out VisualEditor.BackEnd.Input[] cutIns, out VisualEditor.BackEnd.Output[] cutOuts, out Type[] insType, out Vector2[] inPoss, out Vector2[] outPoss)
    {
        Debug.Log("copy for grouping");
        //copy data
        VisualEditor.BackEnd.Node[] buffer = new VisualEditor.BackEnd.Node[selectedNodes.Count];
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        clipboardPos = new Vector2[buffer.Length];
        List<VisualEditor.BackEnd.Input> insR = new List<VisualEditor.BackEnd.Input>();
        List<VisualEditor.BackEnd.Output> outsR = new List<VisualEditor.BackEnd.Output>();
        List<Type> inTypes = new List<Type>();
        List<Vector2> inPos = new List<Vector2>();
        List<Vector2> outPos = new List<Vector2>();

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = selectedNodes[i].GetComponent<NodeVisual>().node;
            clipboardPos[i] = selectedNodes[i].position;
        }
        //duplicate nodes
        bf.Serialize(ms, buffer);
        ms.Position = 0;
        buffer = (VisualEditor.BackEnd.Node[])bf.Deserialize(ms);

        //disconnect inputs and outputs that are not linked to a node inside the selection
        for (int i = 0; i < buffer.Length; i++)
        {
            for (int k = 0; k < buffer[i].inputs.Count; k++)
            {
                if (buffer[i].inputs[k].outputConnectedTo != null && !buffer.Contains(buffer[i].inputs[k].outputConnectedTo.host))
                {
                    insR.Add(buffer[i].inputs[k]);
                    inTypes.Add(buffer[i].inputs[k].GetDataType());
                    inPos.Add(selectedNodes[i].GetComponent<NodeVisual>().transform.position);

                    buffer[i].inputs[k].SetIncommingDataType(null);
                    buffer[i].inputs[k].outputConnectedTo = null;
                    buffer[i].inputs[k].host.PartialSetup();
                }
            }
            for (int k = 0; k < buffer[i].outputs.Count; k++)
            {
                VisualEditor.BackEnd.Input[] copy = buffer[i].outputs[k].destinations.ToArray();

                for (int z = 0; z < copy.Length; z++)
                {
                    if (!buffer.Contains(copy[z].host))
                    {
                        outPos.Add(selectedNodes[i].GetComponent<NodeVisual>().transform.position);
                        buffer[i].outputs[k].DisconnectFrom(copy[z]);
                        outsR.Add(buffer[i].outputs[k]);
                    }
                }
            }
        }
        cutIns = insR.ToArray();
        cutOuts = outsR.ToArray();
        insType = inTypes.ToArray();
        inPoss = inPos.ToArray();
        outPoss = outPos.ToArray();
        clipboard = buffer;
    }

    internal void OnDeleteNode(NodeVisual node)
    {
        if (groupMode)
            hostIfGroup.RemoveNode(node);
    }

    public void Paste()
    {
        if (clipboard == null)
            return;

        List<RectTransform> list = new List<RectTransform>();
        for (int i = 0; i < clipboard.Length; i++)
        {
            //Cree les node visual en settant tout bien les public var puis call CreateLinkVisualsForList() sur allnodes
            Type type = clipboard[i].GetAssociatedVisualClass();
            GameObject go = new GameObject(type.ToString());
            NodeVisual comp = (NodeVisual)go.AddComponent(type);
            comp.host = this;
        //    comp.caster = caster;
            comp.canvas = editorUI;
            comp.doStart = false;
            comp.node = clipboard[i];
            RectTransform rect = comp.gameObject.AddComponent<RectTransform>();
            allNodes.Add(rect);
            list.Add(rect);
            OnNodeCreated(comp);
        }
        StartCoroutine(LoadWait(list, clipboardPos));
        StartCoroutine(LinkVisualCreationWait(list));

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, clipboard);
        ms.Position = 0;
        clipboard = (VisualEditor.BackEnd.Node[])bf.Deserialize(ms);
    }

    bool IsValidFilename(string fileName)
    {
        if (fileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
        {
            return false;
        }
        return true;
    }

    char ValidateInput(string input, int index, char c)
    {
        if (IsValidFilename(c.ToString()))
            return c;
        else
            return '\0';
    }

    void Start () {
        editorUI = Instantiate(Resources.Load<GameObject>("EditorUI")) as GameObject;
        editorUI.transform.SetParent(transform, false);
        overUI = Instantiate(Resources.Load<GameObject>("OverEverythingUI")).GetComponent<Canvas>();
        overUI.transform.SetParent(transform, false);
        scaler = editorUI.GetComponent<CanvasScaler>();

        if (!groupMode)
        {
            GameObject UIGo = Instantiate(Resources.Load<GameObject>("UI Input Manager Global")) as GameObject;
            UIGlobal = UIGo;
            UIGo.GetComponentInChildren<PlayButton>().manager = this;
            activeManager = this;

            Button saveBut = UIGo.transform.FindChild("Save").GetComponent<Button>();
            saveBut.onClick.AddListener(OnClickSave);
            Button autoSave = UIGo.transform.FindChild("AutoSave").GetComponent<Button>();
            autoSave.onClick.AddListener(OnClickAutoSave);
            autoSaveText = autoSave.transform.GetChild(0).GetComponent<Text>();
            Button backToMenu = UIGo.transform.FindChild("back to menu").GetComponent<Button>();
            backToMenu.onClick.AddListener(BackToMenuClick);

        }
        GameObject UIGoLocal = Instantiate(Resources.Load<GameObject>("UI Input Manager Local")) as GameObject;
        UIGoLocal.transform.SetParent(transform, false);

        InputField nameInput = UIGoLocal.transform.FindChild("InputField").GetComponent<InputField>();
        nameInput.onValueChanged.AddListener(OnNameChange);
        nameInput.onValidateInput += ValidateInput;
        nameInput.text = groupMode ? hostIfGroup.groupName : saveName;
        if (groupMode)
        {
            Button back = UIGoLocal.transform.FindChild("Back").GetComponent<Button>();
            back.onClick.AddListener(OnClickBack);
        }
        else
        {
            Destroy(UIGoLocal.transform.FindChild("Back").gameObject);
        }
        


        Transform rBG = editorUI.transform.FindChild("Raycast BG");
        EventTrigger trigger = rBG.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener(OnBGDrag);
        trigger.triggers.Add(dragEntry);

        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener(BGEndDrag);
        trigger.triggers.Add(endDrag);

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener(OnBGClick);
        trigger.triggers.Add(clickEntry);

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener(OnBGPointerDown);
        trigger.triggers.Add(pointerDownEntry);

        VisualEditor.BackEnd.TabMenu.Init();

        //StartCoroutine(BaseSpellWait());
    }

    private void BackToMenuClick()
    {
        for(int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].GetComponent<NodeVisual>().Delete();
        }
        Instantiate(Resources.Load<GameObject>("UIListSpell"));
        if (spell != null)
            PlayStopSpell();
        //TODO Lock manager doesnt get deleted if in game while pressing back to menu

        Destroy(gameObject);
        Destroy(UIGlobal);
    }

    IEnumerator BaseSpellWait()
    {
        yield return new WaitForEndOfFrame();
        if (forceOverrideSave || groupMode || true)
            yield break;
        FileStream file = null;
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.dataPath + "/Spells/Examples/BaseSpell.SpellDic", FileMode.Open);
            InputManagerSerializer dick = (InputManagerSerializer)bf.Deserialize(file);
            clipboard = dick.nodes;
            clipboardPos = dick.PositionsToVector();
            Paste();
            clipboard = null;
            clipboardPos = null;
        }
        catch (Exception exx)
        {
            Debug.LogError(exx.Message + exx.StackTrace);
        }
        finally
        {
            if (file != null)
                file.Close();
        }
    }

    private void OnClickAutoSave()
    {
        autoSave = !autoSave;
        if (!autoSave)
        {
            autoSaveText.text = "Auto Save : off";
            StopCoroutine(AutoSaveCoroutine());
        }
        else
        {
            autoSaveText.text = "Auto Save : on";
            StartCoroutine(AutoSaveCoroutine());
        }
    }

    IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(30f);
            Debug.Log("autosaving");
            
            if (autoSave)
                Save();
            else
                yield break;
        }
    }

    private void BGEndDrag(BaseEventData eventData)
    {
        safeFromDeselect.Clear();
        safeFromSelect.Clear();
        if(selectionSquare != null)
        {
            Destroy(selectionSquare.gameObject);
        }
    }

    private void OnBGDrag(BaseEventData eventData)
    {
        PointerEventData ev = eventData as PointerEventData;
        if (eventData == null)
            return;

        if(ev.button == PointerEventData.InputButton.Right)
        {
            for (int i = 0; i < allNodes.Count; i++)
            {
                allNodes[i].GetComponent<NodeVisual>().OnDragNoCheck(eventData);
            }
        }
        else if(ev.button == PointerEventData.InputButton.Left)
        {            
            if(selectionSquare == null)
            {
                //create the selection square and place it
                GameObject select = new GameObject("Selection square");
                selectionSquare = select.AddComponent<RectTransform>();
                Image img = select.AddComponent<Image>();
                img.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                img.raycastTarget = false;

                selectionSquare.SetParent(overUI.transform, false);
                selectionSquare.pivot = new Vector2(0f, 0f);
                selectionSquare.position = ev.pressPosition;
                selectionSquare.sizeDelta = Vector2.zero;

                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    safeFromDeselect = selectedNodes.ToList();
                }
            }
            else
            {

                Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(selectionSquare);
                bounds.center += selectionSquare.position;
               // Debug.DrawLine((bounds.center - bounds.extents), (bounds.center + bounds.extents), Color.red);
                for(int i = 0; i < allNodes.Count; i++)
                {
                    Bounds nodeBounds = new Bounds(allNodes[i].position, allNodes[i].sizeDelta * targetScaleFactor);
                   // Debug.DrawLine(nodeBounds.center - nodeBounds.extents, nodeBounds.center + nodeBounds.extents, Color.blue);
                    if (bounds.Intersects(nodeBounds))
                    {
                        if (safeFromDeselect.Contains(allNodes[i]))
                        {
                            RemoveFromSelection(allNodes[i]);
                            safeFromSelect.Add(allNodes[i]);
                        }
                        else
                        {
                            if(!safeFromSelect.Contains(allNodes[i]))
                                AddToSelection(allNodes[i]);
                        }
                    }
                    else
                    {
                        if (!safeFromDeselect.Contains(allNodes[i]))
                            RemoveFromSelection(allNodes[i]);
                        if (safeFromSelect.Contains(allNodes[i]))
                        {
                            safeFromSelect.Remove(allNodes[i]);
                            AddToSelection(allNodes[i]);
                        }
                    }
                }
                #region draw square
                //change the size of the selection square
                if (ev.position.x < ev.pressPosition.x && ev.position.y < ev.pressPosition.y)
                {
                    //bottom left
                    selectionSquare.pivot = Vector2.zero;
                    selectionSquare.position = ev.position;
                    selectionSquare.sizeDelta = ev.pressPosition - ev.position;
                }
                else if(ev.position.x > ev.pressPosition.x && ev.position.y < ev.pressPosition.y)
                {
                    //bottom right
                    selectionSquare.pivot = new Vector2(0f, 1f);
                    selectionSquare.position = ev.pressPosition;
                    selectionSquare.sizeDelta = new Vector2(ev.position.x - ev.pressPosition.x, (ev.position.y - ev.pressPosition.y)*-1f);
                }
                else if(ev.position.x > ev.pressPosition.x && ev.position.y > ev.pressPosition.y)
                {
                    //top right
                    selectionSquare.pivot = new Vector2(0f, 1f);
                    selectionSquare.position = new Vector2(ev.pressPosition.x, ev.position.y);
                    selectionSquare.sizeDelta = ev.position - ev.pressPosition;
                }
                else if(ev.position.x < ev.pressPosition.x && ev.position.y > ev.pressPosition.y)
                {
                    //top left
                    selectionSquare.pivot = new Vector2(0f, 1f);
                    selectionSquare.position = ev.position;
                    selectionSquare.sizeDelta = new Vector2(ev.pressPosition.x - ev.position.x, ev.position.y - ev.pressPosition.y);
                }
                #endregion
            }
        }
    }

    private void RemoveFromSelection(RectTransform node)
    {
        if (selectedNodes.Remove(node))
        {
            node.GetComponent<NodeVisual>().SetBGColor(Color.white);
            node.GetComponent<NodeVisual>().selected = false;
        }
    }

    private void AddToSelection(RectTransform node)
    {
        if (selectedNodes.Contains(node))
            return;
        selectedNodes.Add(node);
        node.GetComponent<NodeVisual>().selected = true;
        node.GetComponent<NodeVisual>().SetBGColor(Color.red);//TODO change that to an outline or something
    }

    public void OnBGClick(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (OutputVisual.status == OutputVisual.LINKING)
        {
            OutputVisual.CancelLink();
        }
        for(int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].GetComponent<NodeVisual>().CloseRightClickMenu();
        }
        if (tabMenu != null && tabMenu.gameObject.activeSelf)
        {
            tabMenu.gameObject.SetActive(false);
        }

        //right click menu
        if (splatRightClick != null)
        {
            Destroy(splatRightClick.gameObject);
        }
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            if(selectedNodes.Count != 0)
            {
                StartCoroutine(SplatMenuRightClick());
            }
        }
    }

    IEnumerator SplatMenuRightClick()
    {
        splatRightClick = SplatMenu.Create(allNodes[0].GetComponent<NodeVisual>(), new string[] { "del", "group", "dis" }, OnRightClickSplatSelect, new Vector2(50f, 50f), 70f, true);
        splatRightClick.SetColor(Color.red);
        splatRightClick.transform.position = Input.mousePosition;
        firstSelectSplat = true;
        yield return new WaitForEndOfFrame();
        splatRightClick.transform.SetParent(overUI.transform, true);
        splatRightClick.Open();
    }

    private void OnRightClickSplatSelect(string selected)
    {
        if (firstSelectSplat)
            firstSelectSplat = false;
        else
        {
            Destroy(splatRightClick.gameObject);
            switch (selected)
            {
                case "del":
                    {
                        DeleteSelectedNodes();
                    }
                    break;
                case "group":
                    {
                        //delete the extra links like in copy

                        //group nodes and delete
                        GroupNodeVisual nv = (GroupNodeVisual) NodeVisual.Create(typeof(GroupNodeVisual),
                            this, new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height),
                            editorUI, false);
                        nv.node = nv.GetNode();
                        GroupNode groupNode = (GroupNode)nv.node;
                        nv.initClip = clipboard;
                        nv.initClipPos = clipboardPos;

                        VisualEditor.BackEnd.Input[] ins;
                        VisualEditor.BackEnd.Output[] outs;
                        Type[] inTypes;
                        Vector2[] inPos;
                        Vector2[] outPos;

                        CopySelectedNodesForGrouping(out ins, out outs, out inTypes, out inPos, out outPos);
                        List<VisualEditor.BackEnd.Node> clipTmp = new List<VisualEditor.BackEnd.Node>(clipboard);
                        List<Vector2> clipPosTmp = new List<Vector2>(clipboardPos);
                        //create inputs and outputs group nodes
                        for(int i = 0; i < ins.Length; i++)
                        {//TODO quand la node a un input adaptatif ca marche po
                            GroupInputNode node = new GroupInputNode();
                            node.BeginSetup();
                            node.SetDataType(inTypes[i]);
                            if (node.outputs[0].ConnectToDiscreet(ins[i]))
                            {
                                clipTmp.Add(node);
                                Vector2 pos = inPos[i] - new Vector2(500f, 0f);
                                while (clipPosTmp.Contains(pos))//prevent nodes from being on top of each other
                                {
                                    pos -= new Vector2(0f, 150f);
                                }
                                clipPosTmp.Add(pos);
                            }
                            else
                            {
                                Debug.Log("in not good");
                                node.Delete();
                            }
                        }
                        for(int i = 0; i < outs.Length; i++)
                        {
                            GroupOutputNode node = new GroupOutputNode();
                            node.BeginSetup();
                            if (outs[i].ConnectToDiscreet(node.inputs[0]))
                            {
                                clipTmp.Add(node);
                                Vector2 pos = outPos[i] + new Vector2(500f, 0f);
                                while (clipPosTmp.Contains(pos))
                                {
                                    pos -= new Vector2(0f, 150f);
                                }
                                clipPosTmp.Add(pos);
                            }
                            else
                            {
                                Debug.Log("out not good");
                                node.Delete();
                            }
                        }

                        InputManager.clipboard = clipTmp.ToArray();
                        InputManager.clipboardPos = clipPosTmp.ToArray();

                        groupNode.BeginSetup();//manually startup the node

                        //delete selected since they have been placed in the group
                        RectTransform[] copy = selectedNodes.ToArray();
                        for (int i = 0; i < copy.Length; i++)
                        {
                            copy[i].GetComponent<NodeVisual>().Delete();
                        }
                    }
                    break;
                case "dis":
                    {
                        for(int i = 0; i < selectedNodes.Count; i++)
                        {
                            NodeVisual n = selectedNodes[i].GetComponent<NodeVisual>();
                            n.SetEnable(!n.node.enabled);
                        }
                    }
                    break;
            }
            
        }
    }

    private void OnBGPointerDown(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (pointerData == null)
            return;

        if (pointerData.button == PointerEventData.InputButton.Left && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            RectTransform[] copy = selectedNodes.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                RemoveFromSelection(copy[i]);
            }
        }
    }

    public void DragSelectedNodes(PointerEventData eventData, RectTransform exceptMe)
    {
        for(int i = 0; i < selectedNodes.Count; i++)
        {
            if(selectedNodes[i] != exceptMe)
            {
                selectedNodes[i].GetComponent<NodeVisual>().OnDragNoCheck(eventData);
            }
        }
    }

	void Update () {
        if (hidden)
            return;

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
        {
            CopySelectedNodes();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.X))
        {
            CopySelectedNodes();
            DeleteSelectedNodes();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.V))
        {
            Paste();
        }

        if(groupMode && !IsInputFieldFocused() && Input.GetKeyDown(KeyCode.Backspace))
        {
            OnClickBack();
        }

        #region shiftClick
        if (wasInShiftingLink && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
            if(OutputVisual.linkInitiator != null && OutputVisual.linkInitiator.pendingLink != null)
            {
                OutputVisual.linkInitiator.pendingLink.FollowMouse(OutputVisual.linkInitiator.gameObject);
                OutputVisual.linkInitiator.pendingLink.keepAlive = false;
            }
            wasInShiftingLink = false;
            bufferInputs = null;
            if (oldTarget != null)
                oldTarget.HideName();
            oldTarget = null;                  
        }

        if(wasInShiftingLink && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)))
        {
            if(oldTarget != null && OutputVisual.linkInitiator != null && OutputVisual.linkInitiator.pendingLink != null)
            {
                OutputVisual.linkInitiator.pendingLink.keepAlive = false;
                OutputVisual.CompleteLink(OutputVisual.linkInitiator.pendingLink.inputDestination);
            }
            wasInShiftingLink = false;
            bufferInputs = null;
            if(oldTarget != null)
                oldTarget.HideName();
            oldTarget = null;
        }

        if (OutputVisual.status == OutputVisual.LINKING && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Vector2.Distance(Input.mousePosition, oldMousePos) > 10)
        {
            if (!wasInShiftingLink)
            {
                wasInShiftingLink = true;
                bufferInputs = FindObjectsOfType<InputVisual>();
            }

            oldMousePos = Input.mousePosition;
            InputVisual closest = null;
            for(int i = 0; i < bufferInputs.Length; i++)
            {
                if (VisualEditor.BackEnd.Output.ContainsType(bufferInputs[i].inputAttachedTo.GetAllowedDataTypes(), OutputVisual.linkInitiator.outputAttachedTo.GetDataType())
                  //  bufferInputs[i].inputAttachedTo.GetAllowedDataTypes().Contains(OutputVisual.linkInitiator.outputAttachedTo.GetDataType()) 
                    && bufferInputs[i].host != OutputVisual.linkInitiator.host
                    && bufferInputs[i].inputAttachedTo.outputConnectedTo == null)
                {
                    if(closest == null)
                    {
                        closest = bufferInputs[i];
                    }
                    else
                    {
                        if (Vector2.Distance(bufferInputs[i].transform.position, Input.mousePosition) < Vector2.Distance(closest.transform.position, Input.mousePosition))
                        {
                            closest = bufferInputs[i];
                        }
                    }
                }
                
            }
            if(oldTarget != closest)
            {
                if(oldTarget != null)
                    oldTarget.HideName();
                if(closest != null)
                    closest.ShowName();
            }
            oldTarget = closest;
            if(closest != null)
            {
                OutputVisual.linkInitiator.pendingLink.keepAlive = true;
                OutputVisual.linkInitiator.pendingLink.FinishFollowingMouse(closest.gameObject);
            }
        }
#endregion

        //en mode pro : quand on clique dans le vide ca ouvre la liste
        //en mode debutant : quand on clique and drag dans le vide ca bouge tous les swag
        if (proMode)
        {
            if (Input.GetMouseButtonDown(2))
            {
                lastMousePos = Input.mousePosition;
                SetNodesAnchorAtMousePos();
            }
            else if (Input.GetMouseButton(2))
            {
                Vector2 newMPos = Input.mousePosition;
                MoveBy(newMPos - lastMousePos);
                lastMousePos = newMPos;
            }
        }
        else
        {//debutant mode

        }

	    if((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Space)) && !IsInputFieldFocused())
        {
            //TODO open list of possible nodes based on if you click on an input, an ouput or nothing
            PopTabMenu();
        }
        if(Input.mouseScrollDelta.y != 0f && (tabMenu == null || !tabMenu.gameObject.activeSelf))
        {
            ZoomBy(Input.mouseScrollDelta.y);
        }
        if (scaler.scaleFactor != targetScaleFactor)
            scaler.scaleFactor = Mathf.Lerp(scaler.scaleFactor, targetScaleFactor, 0.1f);
	}

    public static bool IsInputFieldFocused()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        return obj != null && obj.GetComponent<InputField>() != null && obj.GetComponent<InputField>().isFocused;
    }

    void ResetNodes()
    {
        for(int i = 0; i < allNodes.Count; i++)
        {
            VisualEditor.BackEnd.Node node = allNodes[i].GetComponent<NodeVisual>().node;
            node.Reset();
        }
    }

    public VisualEditor.BackEnd.Spell GetSpell()
    {
        VisualEditor.BackEnd.Node[] nodes = new VisualEditor.BackEnd.Node[allNodes.Count];
        for(int i = 0; i < allNodes.Count; i++)
        {
            nodes[i] = allNodes[i].GetComponent<NodeVisual>().node;
        }
        VisualEditor.BackEnd.Spell spell = new VisualEditor.BackEnd.Spell(nodes, saveName);
        return spell;
    }

    void PopTabMenu()
    {
        if (hidden)
            return;
        if (tabMenu == null)
        {
            //create tab menu
            tabMenu = new GameObject("TabMenu").AddComponent<TabMenuList>();
            tabMenu.pos = Input.mousePosition;
            tabMenu.parent = overUI;
            tabMenu.spawnNodeIn = editorUI.GetComponent<Canvas>();
            tabMenu.host = this;
            tabMenu.UpdateList((Dictionary<string, Type> )VisualEditor.BackEnd.TabMenu.GetListForString(""));
            if (groupMode)
            {
                tabMenu.onNodeCreated += OnNodeCreated;
            }
        }
        else
        {
            //move tab menu
            tabMenu.Show(Input.mousePosition);
        }
    }

    public void OnNodeCreated(NodeVisual node)
    {
        if(groupMode)
            hostIfGroup.AddNode(node);
    }

    public void SetNodesAnchorAtMousePos()
    {
        SetNodesAnchorAtPos(Input.mousePosition);
        /*Vector2 mousePercent = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].anchorMax != mousePercent)
            {
                Vector3 pos = allNodes[i].position;//save the pos

                allNodes[i].anchorMax = mousePercent;
                allNodes[i].anchorMin = mousePercent;

                allNodes[i].position = pos;//re apply the same position so that the thing doesn't move on screen
            }
        }*/
    }

    public void SetNodesAnchorAtPos(Vector2 pos)
    {
        Vector2 posPercent = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].anchorMax != posPercent)
            {
                Vector3 oldPos = allNodes[i].position;//save the pos

                allNodes[i].anchorMax = posPercent;
                allNodes[i].anchorMin = posPercent;

                allNodes[i].position = oldPos;//re apply the same position so that the thing doesn't move on screen
            }
        }
    }

    private void DeleteSelectedNodes()
    {
        RectTransform[] copy = selectedNodes.ToArray();
        for (int i = 0; i < copy.Length; i++)
        {
            copy[i].GetComponent<NodeVisual>().Delete();
        }
    }

    void ZoomBy(float zoomAdd)
    {
        SetNodesAnchorAtMousePos();
        if (targetScaleFactor + zoomAdd * 0.1f > 2.7f)
            targetScaleFactor = 2.7f;
        else if (targetScaleFactor + zoomAdd * 0.1f < 0.1f)
            targetScaleFactor = 0.1f;
        else
            targetScaleFactor += zoomAdd * 0.1f;
    }

    public void MoveBy(Vector2 posAdd)
    {
        Vector2 mousePercent = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        for (int i = 0; i < allNodes.Count; i++)
        {
            Vector2 moveBy = Vector2.Lerp(allNodes[i].anchorMax, mousePercent, 0.1f);
            allNodes[i].anchorMax = moveBy;
            allNodes[i].anchorMin = moveBy;
        }
    }

    private GameObject testPlayer;
    private GameObject testTerrain;
    private SpellInGameManager testSpellManager;

    internal void PlayStopSpell()
    {
        if (spell == null)
        {
            spell = GetSpell();
            //hide UI and spawn player
            activeManager.Hide();
            testPlayer = Instantiate(Resources.Load<GameObject>("Player local"));
            testTerrain = Instantiate(Resources.Load<GameObject>("Test terrain"));
            Camera.main.GetComponent<CameraControl>().UpdatePlayerList();

            testSpellManager = SpellInGameManager.Create(new VisualEditor.BackEnd.Spell[] { spell });
        }
        else
        {
            Destroy(testPlayer);
            Destroy(testTerrain);
            Destroy(testSpellManager.gameObject);
            activeManager.Show();
            spell.Stop();
            spell = null;
            ResetNodes();
        }
    }
}

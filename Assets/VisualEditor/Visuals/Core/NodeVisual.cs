using UnityEngine;
using System.Collections.Generic;
using System;
using VisualEditor.BackEnd;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

namespace VisualEditor.Visuals
{
    public abstract class NodeVisual : MonoBehaviour
    {
        public Vector2 startPosInPercent;//this has to be set when the monobehaviour/gameobject get created
        public GameObject canvas;//this also has to be set when creating, this is the parent in which to place the node (UI canvas)
        public bool selected = false;
        public InputManager host;
        public bool doStart = true;

        public Node node;//this has to be set on Start(), this is given in GetNode() by the implementing class
        protected RectTransform rectTransform;
        protected Image background;//the background image of this node
        public InputVisual[] inputsVisuals;//list of all the visual that should represent the inputs/outputs
        public OutputVisual[] outputsVisuals;

        private List<Type>[] oldInputTypes;//used to check if the inputs/outputs changed and update them only when necessary
        private Type[] oldOutputTypes;
        private SplatMenu splatRightClick;
        private InputField commentText;
        private RectTransform commentCache;
        private float lastClickComment = -10f;
        private bool firstSelect = true;

        internal abstract Node GetNode();
        internal abstract void BeginPersonnalizeSetup();//called just after BeginSetup()
        internal abstract void PersonnalizeSetup();//this is called just after BuildSetup() on Update(), let's you put extra UI stuff in the node
        internal abstract float GetWidth();
        internal abstract float GetHeight();
        internal abstract string GetDisplayName();//name displayed on the node
        internal abstract string GetInputLabel(int inputIndex);//name displayed next to the input/output onhover
        internal abstract string GetOutputLabel(int outputIndex);
        protected abstract void DoInitFromNode();

        public void InitFromNode()
        {
            DoInitFromNode();
            PlaceCommentText();
            SetEnable(node.enabled);
        }

        private void UpdateCommentText()
        {
            commentText.text = node.GetComment();
            //Update size
        }
        
        protected void PlaceCommentText()
        {
            RectTransform rect = commentText.GetComponent<RectTransform>();

            for(int i = 0; i < commentText.text.Length; i++)
            {
                float width = LayoutUtility.GetPreferredWidth(commentText.textComponent.rectTransform);
                rect.sizeDelta = new Vector2(width + 30f, 40f);
            }
            rect.anchoredPosition = new Vector2(0f, -GetHeight() / 2f - (rect.sizeDelta.y / 2f) + 10f);

            commentCache.sizeDelta = rect.sizeDelta;
            commentCache.anchoredPosition = rect.anchoredPosition;
        }

        private void OnCommentEdit(string str)
        {
            node.SetComment(str);
            PlaceCommentText();
        }

        private void OnCommentEndEdit(string str)
        {
            commentCache.gameObject.SetActive(true);
        }

        private void CreateCommentText()
        {
            GameObject fi = Instantiate(Resources.Load<GameObject>("InputField"));
            fi.transform.SetParent(transform, false);
            fi.name = "Comment";

            commentText = fi.GetComponent<InputField>();
            commentText.textComponent.alignment = TextAnchor.MiddleCenter;
            commentText.textComponent.fontStyle = FontStyle.Italic;
          //  commentText.textComponent.fontSize = 20;
            commentText.characterLimit = 100;
            commentText.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            commentText.caretWidth = 3;
            commentText.onValueChanged.AddListener(OnCommentEdit);
            commentText.onEndEdit.AddListener(OnCommentEndEdit);

            GameObject cache = new GameObject("Click");
            cache.transform.SetParent(transform, false);
            cache.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            EventTrigger trigger = cache.AddComponent<EventTrigger>();

            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(ClickCommentCache);
            trigger.triggers.Add(clickEntry);

            EventTrigger.Entry dragEntry = new EventTrigger.Entry();
            dragEntry.eventID = EventTriggerType.Drag;
            dragEntry.callback.AddListener(OnDrag);
            trigger.triggers.Add(dragEntry);

            commentCache = cache.GetComponent<RectTransform>();

            Destroy(fi.GetComponent<Image>());
            Destroy(commentText.placeholder);
        }

        private void ClickCommentCache(BaseEventData ev)
        {
            PointerEventData evv = ev as PointerEventData;
            if (evv == null)
                return;
            if (evv.dragging)
                return;

            lastClickComment = Time.time;

            switch (evv.clickCount)
            {
                case 1:
                    StopCoroutine("DoubleCheckComment");
                    StartCoroutine(DoubleCheckComment(UnityEngine.Input.mousePosition));
                    break;
                case 2:
                    StartComment();
                    break;
            }
        }

        IEnumerator DoubleCheckComment(Vector2 pos)
        {
            yield return new WaitForSeconds(0.2f);
            if(Time.time - lastClickComment >= 0.2f)
            {
                MouseToast.MakeToastFixed(1f, "Double click to edit", pos);
            }
        }
        
        private void StartComment()
        {
            commentText.ActivateInputField();
            commentText.Select();
            commentCache.gameObject.SetActive(false);
        }

        public static NodeVisual Create(Type type, InputManager host, Vector2 startPosInPercent, GameObject canvas, bool doStart)
        {
            GameObject go = new GameObject(type.ToString());
            NodeVisual nv = (NodeVisual) go.AddComponent(type);
            nv.host = host;
            nv.startPosInPercent = startPosInPercent;
            nv.canvas = canvas;
            nv.doStart = doStart;
            host.allNodes.Add(go.AddComponent<RectTransform>());
            host.OnNodeCreated(nv);
            return nv;
        }

        protected virtual void Start()//you cans override that to change the defaults value of the node, don't forget to call base.Start() if you do so
        {
            if (doStart)
            {
                node = GetNode();
                node.BeginSetup();//setup the node 
            }

            BeginBuildLayout();//and build le base layout
            BeginPersonnalizeSetup();//and the personnalized one
        }

        float lastShake = -10f;
        int shakeCount = 0;
        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData pointerData = eventData as PointerEventData;
            if (pointerData == null || pointerData.button != PointerEventData.InputButton.Left) { return; }

            if (pointerData.delta.sqrMagnitude > 7000f)
            {
                if (Time.time - lastShake > 0.3f)
                    shakeCount = 0;
                else
                    shakeCount++;
                lastShake = Time.time;
                if(shakeCount > 5)
                {
                    DisconnectEverything();
                    shakeCount = 0;
                }
            }
            rectTransform.position += (Vector3)pointerData.delta;
            if (selected)
                host.DragSelectedNodes(pointerData, rectTransform);
            //if this node is selected, relay the event to the input manager so he can relay it to the other selected nodes
                
        }

        public void OnDragNoCheck(BaseEventData eventData)
        {
            PointerEventData pointerData = eventData as PointerEventData;
            if (pointerData == null)
                return; 
            rectTransform.position += (Vector3)pointerData.delta;
        }

        protected virtual void Update()//update the node if necessary
        {//can be overriden but be careful to either call base.Update() or do your shit
            BuildLayout();
            PersonnalizeSetup();
        }

        protected virtual void BeginBuildLayout()//called once on start, setup the ground UI work
        {
            rectTransform = GetComponent<RectTransform>();//get the recttransform

            rectTransform.SetParent(canvas.transform, false);//setup the position on the mouse's position
            rectTransform.anchorMin = startPosInPercent;
            rectTransform.anchorMax = startPosInPercent;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(GetWidth(), GetHeight());

            CreateText();

            background = gameObject.AddComponent<Image>();//background image
            EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(OnClick);
            trigger.triggers.Add(clickEntry);

            EventTrigger.Entry dragEntry = new EventTrigger.Entry();//listen to drag event
            dragEntry.eventID = EventTriggerType.Drag;
            dragEntry.callback.AddListener(OnDrag);
            trigger.triggers.Add(dragEntry);

            BuildInputs();
            BuildOutputs();
            CreateCommentText();
            UpdateCommentText();
            PlaceCommentText();
        }

        public void OnClick(BaseEventData eventData)
        {
            PointerEventData ev = eventData as PointerEventData;
            if (ev == null)
                return;

            if(ev.button == PointerEventData.InputButton.Right)
            {
                if (selected)
                {
                    //relay to host
                    host.OnBGClick(eventData);
                }
                else if(splatRightClick == null)
                {
                    StartCoroutine(RightClick());
                }
            }
        }

        IEnumerator RightClick()
        {
            splatRightClick = SplatMenu.Create(this, new string[] { "doc", "del", "dis", "com" }, OnSelectedRightClick, new Vector2(50f, 50f), 70f, true);
            splatRightClick.SetColor(Color.red);
            splatRightClick.transform.position = UnityEngine.Input.mousePosition;
            firstSelect = true;
            yield return new WaitForEndOfFrame();
            splatRightClick.transform.SetParent(host.overUI.transform, true);
            splatRightClick.Open();
        }

        private void OnSelectedRightClick(string s)
        {
            if (firstSelect)
                firstSelect = false;
            else
            {
                Destroy(splatRightClick.gameObject);
                switch (s)
                {
                    case "doc":
                        //blabla
                        //
                        //Possible input types: blabla
                        //Possible output types: blabla
                        StringBuilder str = new StringBuilder(node.GetDocumentation() + "\n\n");
                        Type[] inputTypes = node.GetPossibleInputTypes();
                        Type[] outputTypes = node.GetPossibleOutputTypes();
                        InputManagerSerializer example = node.GetExample();

                        if (inputTypes.Length != 0)
                        {
                            str.Append("Possible input types: ");
                            for (int i = 0; i < inputTypes.Length; i++)
                            {
                                string cleanName = TabMenu.CleanClassName(inputTypes[i].ToString());
                                str.Append(cleanName);
                                if (i != inputTypes.Length - 1)
                                    str.Append(", ");
                            }
                        }

                        if(outputTypes.Length != 0)
                        {
                            str.Append("\nPossible output types: ");
                            for (int i = 0; i < outputTypes.Length; i++)
                            {
                                string cleanName = TabMenu.CleanClassName(outputTypes[i].ToString());
                                str.Append(cleanName);
                                if (i != outputTypes.Length - 1)
                                    str.Append(", ");
                            }
                        }
                        if(example == null)
                        {
                            MsgBox.Make(str.ToString());
                        }
                        else
                        {
                            MsgBox.Make(str.ToString(), new string[] { "Paste an example" }, new MsgBox.OnButtonClick[] {
                                (MsgBox msg, object o) => {
                                    InputManagerSerializer exem = (InputManagerSerializer)o;
                                    InputManager.clipboard = exem.nodes;
                                    InputManager.clipboardPos = exem.PositionsToVector();
                                    host.Paste();
                                    msg.Close();
                                } }, new object[] { example });
                        }
                        break;
                    case "del":
                        Delete();
                        break;
                    case "dis":
                        SetEnable(!node.enabled);
                        break;
                    case "com":
                        StartComment();
                        break;
                }
            }
        }

        public void SetEnable(bool enabled)
        {
            node.enabled = enabled;
            if (!enabled)
            {
                SetAlpha(0.3f);
            }
            else
            {
                SetAlpha(1f);
            }
        }

        public void SetBGColor(Color c)
        {
            background.color = new Color(c.r, c.g, c.b, background.color.a);
        }

        private void SetAlpha(float alpha)
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, alpha);
        }

        public void CloseRightClickMenu()
        {
            if (splatRightClick != null)
                Destroy(splatRightClick.gameObject);
        }

        public void Delete()
        {
            host.OnDeleteNode(this);
            DisconnectAllInputs();
            DisconnectAllOutputs();
            node.Delete();
            if (selected)
                host.selectedNodes.Remove(rectTransform);
            host.allNodes.Remove(rectTransform);
            Destroy(gameObject);
        }

        public  virtual void BuildLayout()//this is called on Update() so it has to be optimized and not redraw things unecessaraly
        {
            
            //if the count of input/output has change no need to check if thet are different, go though them and up/create/delete to match de node's input/output
            //otherwise go though each input/ouputs and check if their type has changed, if so update them
            //update oldInputtypes and oldOuputtypes
            if(node.inputs.Count != oldInputTypes.Length)
            {
                InputsCountChanged();
            }
            else
            {
                CheckAndUpdateInputsIfNecessary();
            }

            if(node.outputs.Count != oldOutputTypes.Length)
            {
                OutputsCountChanged();
            }
            else
            {
                CheckAndUpdateOutputsIfNecessary();
            }
        }

        void BuildInputs()
        {
            //Debug.Log("Building inputs for " + GetType());

            inputsVisuals = new InputVisual[node.inputs.Count];
            oldInputTypes = new List<Type>[inputsVisuals.Length];

            for (int i = 0; i < node.inputs.Count; i++)//inputs
            {
                InputVisual inp = CreateInputVisual(node.inputs[i], GetInputLabel(i));
                inp.name = "input_" + i;
                inputsVisuals[i] = inp;

                List<Type> dataTypes = GetDataTypeOrAllowed(node.inputs[i]);
                inp.UpdateType();
                oldInputTypes[i] = dataTypes;
            }
            PlaceInputs();
        }

        void BuildOutputs()
        {
            //Debug.Log("Building outputs for " + GetType());

            outputsVisuals = new OutputVisual[node.outputs.Count];
            oldOutputTypes = new Type[outputsVisuals.Length];

            for (int i = 0; i < node.outputs.Count; i++)//outputs
            {
                OutputVisual outp = CreateOutputVisual(node.outputs[i], GetOutputLabel(i));
                outp.name = "output_" + i;
                outp.UpdateType();

                outputsVisuals[i] = outp;
                oldOutputTypes[i] = node.outputs[i].GetDataType();
            }
            PlaceOutputs();
        }

        void CheckAndUpdateInputsIfNecessary()
        {
            List<Type> dataType = null;
            for (int i = 0; i < node.inputs.Count; i++)
            {
                dataType = GetDataTypeOrAllowed(node.inputs[i]);
                if(!ListsAreTheSame(dataType, oldInputTypes[i]))
                {
                    UpdateInput(i);
                }
            }
        }

        void CheckAndUpdateOutputsIfNecessary()
        {
            for (int i = 0; i < node.outputs.Count; i++)
            {
                if (node.outputs[i].GetDataType() != oldOutputTypes[i])
                {
                    UpdateOutput(i);
                }
            }
        }

        public void UpdateLabels()
        {
            for(int i = 0; i < inputsVisuals.Length; i++)
            {
                inputsVisuals[i].SetLabel(GetInputLabel(i));
            }
            for (int i = 0; i < outputsVisuals.Length; i++)
            {
                outputsVisuals[i].SetLabel(GetOutputLabel(i));
            }
        }

        void UpdateInput(int i)
        {
            List<Type> dataTypes = GetDataTypeOrAllowed(node.inputs[i]);
            inputsVisuals[i].UpdateType();//update the visual
            oldInputTypes[i] = dataTypes;//and the old 
            inputsVisuals[i].inputAttachedTo = node.inputs[i];
            inputsVisuals[i].UpdateType();
            inputsVisuals[i].SetLabel(GetInputLabel(i));
            if (inputsVisuals[i].outputConnectedTo != null && !Output.ContainsType(inputsVisuals[i].inputAttachedTo.GetAllowedDataTypes(), inputsVisuals[i].outputConnectedTo.outputAttachedTo.GetDataType()))
                inputsVisuals[i].Disconnect();
            //Debug.Log("updating input " + i + " on " + GetType());//leave that in for testing if the inputs dont get updated constantly
        }

        void UpdateOutput(int i)
        {
            outputsVisuals[i].UpdateType();
            oldOutputTypes[i] = node.outputs[i].GetDataType();
            outputsVisuals[i].outputAttachedTo = node.outputs[i];
            outputsVisuals[i].UpdateType();
            outputsVisuals[i].SetLabel(GetOutputLabel(i));
            //Debug.Log("updating output " + i + " on " + GetType());
        }

        void InputsCountChanged()
        {
            //go though each input, if the old input existed update it if not create one
            //an then delete all the inputs in surplus
            List<InputVisual> nInputVisuals = new List<InputVisual>();
            List<List<Type>> noldInputTypes = new List<List<Type>>();
            for(int i = 0; i < node.inputs.Count; i++)
            {
                if(inputsVisuals.Length > i)
                {
                    //if there is and old input here 
                    //update it
                    UpdateInput(i);
                    nInputVisuals.Add(inputsVisuals[i]);
                    noldInputTypes.Add(GetDataTypeOrAllowed(node.inputs[i]));
                }
                else
                {
                    //if there is not and old input here
                    //create input
                    InputVisual inp = CreateInputVisual(node.inputs[i], GetInputLabel(i));
                    inp.name = "input_" + i;

                    List<Type> dataType = GetDataTypeOrAllowed(node.inputs[i]);
                    inp.UpdateType();

                    PlaceInput(inp, i);
                    nInputVisuals.Add(inp);
                    noldInputTypes.Add(dataType);
                }
            }
            if(inputsVisuals.Length > node.inputs.Count)
            {
                //there is some input in surplus
                //delete them (the surplus)
                for(int i = node.inputs.Count; i < inputsVisuals.Length; i++)
                {
                    inputsVisuals[i].Disconnect();
                    Destroy(inputsVisuals[i].gameObject);
                }
            }
            //resize oldInputTypes and inputVisuals
            inputsVisuals = nInputVisuals.ToArray();
            oldInputTypes = noldInputTypes.ToArray();
        }

        void OutputsCountChanged()
        {
            List<OutputVisual> nOutputVisuals = new List<OutputVisual>();
            List<Type> nOldOutputTypes = new List<Type>();
            for(int i = 0; i < node.outputs.Count; i++)
            {
                if(outputsVisuals.Length > i)
                {
                    //if there is an old output here 
                    //update it
                    UpdateOutput(i);
                    nOutputVisuals.Add(outputsVisuals[i]);
                    nOldOutputTypes.Add(node.outputs[i].GetDataType());
                }
                else
                {
                    //if there is not and old outpute here
                    //create output
                    OutputVisual outp = CreateOutputVisual(node.outputs[i], GetOutputLabel(i));
                    outp.name = "output_" + i;
                    outp.UpdateType();

                    PlaceOutput(outp, i);
                    nOutputVisuals.Add(outp);
                    nOldOutputTypes.Add(node.outputs[i].GetDataType());
                }
            }
            if (outputsVisuals.Length > node.outputs.Count)
            {
                //there is some output in surplus
                //delete them (the surplus)
                for (int i = node.outputs.Count; i < outputsVisuals.Length; i++)
                {
                    outputsVisuals[i].Disconnect();
                    Destroy(outputsVisuals[i].gameObject);
                }
            }
            outputsVisuals = nOutputVisuals.ToArray();
            oldOutputTypes = nOldOutputTypes.ToArray();
        }

        List<Type> GetDataTypeOrAllowed(BackEnd.Input inp)
        {
            if(inp.GetDataType() == null)//several data type; we don't know which data type this input is taking 
            {
                return inp.GetAllowedDataTypes();
            }
            else//only one data type
            {
                return new List<Type>(new Type[] { inp.GetDataType() });
            }
        }

        InputVisual CreateInputVisual(BackEnd.Input input, string label)
        {
            GameObject go = new GameObject();
            go.AddComponent<RectTransform>();
            go.transform.SetParent(rectTransform, false);
            InputVisual inp = go.AddComponent<InputVisual>();
            inp.inputAttachedTo = input;
            inp.host = this;
            inp.SetLabel(label);
            return inp;
        }

        OutputVisual CreateOutputVisual(Output output, string label)
        {
            GameObject go = new GameObject();
            go.AddComponent<RectTransform>();
            go.transform.SetParent(rectTransform, false);
            OutputVisual outp = go.AddComponent<OutputVisual>();
            outp.outputAttachedTo = output;
            outp.host = this;
            outp.SetLabel(label);
            return outp;
        }

        void DisconnectEverything()
        {
            DisconnectAllInputs();
            DisconnectAllOutputs();
        }

        void DisconnectAllInputs()
        {
            for(int i = 0; i < inputsVisuals.Length; i++)
            {
                inputsVisuals[i].Disconnect();
            }
        }

        void DisconnectAllOutputs()
        {
            for (int i = 0; i < outputsVisuals.Length; i++)
            {
                outputsVisuals[i].Disconnect();
            }
        }

        protected virtual void PlaceInputs()
        {
            for(int i = 0; i < inputsVisuals.Length; i++)
            {
                PlaceInput(i);
            }
        }

        protected virtual void PlaceOutputs()
        {
            for (int i = 0; i < outputsVisuals.Length; i++)
            {
                PlaceOutput(i);
            }
        }

        void PlaceInput(InputVisual inp, int i)
        {
            inp.rectTransform.anchoredPosition = new Vector2(-GetWidth() / 2f, (GetHeight() / 2f) - (25f * i) - 20f);
        }

        void PlaceOutput(OutputVisual outp, int i)
        {
            outp.rectTransform.anchoredPosition = new Vector2(+GetWidth() / 2f, (GetHeight() / 2f) - (25f * i) - 20f);
        }

        void PlaceInput(int i)
        {
            inputsVisuals[i].rectTransform.anchoredPosition = new Vector2(-GetWidth() / 2f, (GetHeight() / 2f) - (25f * i) - 20f);
        }

        void PlaceOutput(int i)
        {
            outputsVisuals[i].rectTransform.anchoredPosition = new Vector2(+GetWidth() / 2f, (GetHeight() / 2f) - (25f * i) - 20f);
        }

        protected virtual void CreateText()//create the text to display the name of the node
        {//this can be overriden, please stay consitant 
            GameObject textObj = new GameObject("display text");
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.SetParent(rectTransform, false);
            rect.anchoredPosition = new Vector2(0f, GetHeight()/2 + 20f);

            Text t = textObj.AddComponent<Text>();
            t.font = Font.CreateDynamicFontFromOSFont("Lato", 30);//TODO change the font to a font in the ressource folder, and cache it 
            t.text = GetDisplayName();
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.black;
            t.raycastTarget = false;
            t.fontSize = 30;
            t.fontStyle = FontStyle.Bold;
        }

        static bool ListsAreTheSame<T>(List<T> t1, List<T> t2)
        {
            if (t1.Count != t2.Count)
                return false;
            for (int i = 0; i < t1.Count; i++)
            {
                if ((object)t1[i] != (object)t2[i])
                    return false;
            }
            return true;
        }
    }
}

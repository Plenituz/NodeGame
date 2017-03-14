using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VisualEditor.BackEnd;
using System.Text;

namespace VisualEditor.Visuals
{
    public class OutputVisual : MonoBehaviour
    {
        public static int status = 0;//indicates if you are curently linking or idling
        public static OutputVisual linkInitiator;

        public const int IDLE = 0;
        public const int LINKING = 1;

        public RectTransform rectTransform;
        public List<LinkVisual> links = new List<LinkVisual>();//list of all the links already created
        public LinkVisual pendingLink;//the link getting created if there is one, this is set by the static methods
        public Output outputAttachedTo;//this has to be set on creation of this Gameobject
        public NodeVisual host; //Set on clreating this gameobject
        public Text label;

        private Image img;
        private bool stay = false;

        void Awake()
        {
            //initialise the UI elements
            //if it's an ouput flip the sprite
            rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(20f, 20f);
            img = gameObject.AddComponent<Image>();

            GameObject lbl = Instantiate(Resources.Load<GameObject>("BaseText")) as GameObject;
            lbl.transform.SetParent(transform, false);
            label = lbl.GetComponent<Text>();
            label.alignment = TextAnchor.MiddleLeft;
            label.fontSize = 17;
            label.rectTransform.anchoredPosition = new Vector2(93f, 0f);
            label.raycastTarget = false;
           // label.CrossFadeAlpha(0f, 1f, true);
           
            EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry clickEntry = new EventTrigger.Entry();//listen to drag event
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(OnClick);
            trigger.triggers.Add(clickEntry);

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();//listen to drag event
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener(PointerEnter);
            trigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();//listen to drag event
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener(PointerExit);
            trigger.triggers.Add(pointerExitEntry);

            EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();//listen to drag event
            beginDragEntry.eventID = EventTriggerType.BeginDrag;
            beginDragEntry.callback.AddListener(OnDragBegin);
            trigger.triggers.Add(beginDragEntry);

            EventTrigger.Entry endDragEntry = new EventTrigger.Entry();//listen to drag event
            endDragEntry.eventID = EventTriggerType.EndDrag;
            endDragEntry.callback.AddListener(OnDragEnd);
            trigger.triggers.Add(endDragEntry);
        }

        private void PointerEnter(BaseEventData eventData)
        {
            PointerEventData ev = eventData as PointerEventData;
            if (ev == null)
                return;


            if (!stay)
                label.CrossFadeAlpha(1f, 0.3f, true);
        }

        private void PointerExit(BaseEventData eventData)
        {
            PointerEventData ev = eventData as PointerEventData;
            if (ev == null)
                return;
            if (UnityEngine.Input.GetMouseButton(1))
            {
                stay = !stay;
            }
            if (!stay)
                label.CrossFadeAlpha(0f, 0.2f, true);
        }

        private void OnDragBegin(BaseEventData arg0)
        {
            PointerEventData ev = arg0 as PointerEventData;
            if (ev == null)
                return;

            if(ev.button == PointerEventData.InputButton.Left)
            {
                if(status == IDLE)
                {
                    StartLinking(this);
                }
            }
        }

        private void OnDragEnd(BaseEventData arg0)
        {
            PointerEventData ev = arg0 as PointerEventData;
            if (ev == null)
                return;

            if (status == LINKING && EventSystem.current.IsPointerOverGameObject())
            {
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(ev, results);
                bool good = false;
                for(int i = 0; i < results.Count; i++)
                {
                    InputVisual r = results[i].gameObject.GetComponent<InputVisual>();
                    if (r != null && linkInitiator.host != r.host)
                    {
                        r.Disconnect();
                        CompleteLink(r);
                        good = true;
                        break;
                    }
                }
                if (!good && !(UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)))
                    CancelLink();
            }
        }

        public void UpdateType()
        {
            //change the color/shape of the input base on the type given
            img.color = GetColorForType(outputAttachedTo.GetDataType());
        }

        private void OnClick(BaseEventData arg0)
        {
            PointerEventData ev = arg0 as PointerEventData;
            if (ev == null)
                return;
            
            if(ev.button == PointerEventData.InputButton.Left)
            {
                if (ev.clickCount == 2)
                {
                    if (status == LINKING)
                        CancelLink();
                    Disconnect();
                }
                if (status == IDLE)
                {
                    StartLinking(this);
                }
                else if (linkInitiator == this)
                {
                    CancelLink();
                }
            }
            else if(ev.button == PointerEventData.InputButton.Right)
            {
                stay = !stay;
            }
            else if(ev.button == PointerEventData.InputButton.Middle)
            {
                Type type = outputAttachedTo.GetDataType();
                MouseToast.MakeToastFixed(2f, "Type: " + TabMenu.CleanClassName(type.ToString()), UnityEngine.Input.mousePosition);
            }
        }

        public static void StartLinking(OutputVisual linkStart)
        {
            linkInitiator = linkStart;
            status = LINKING;

            GameObject link = new GameObject(linkStart.name + "_link_" + linkStart.links.Count);
            linkStart.pendingLink = link.AddComponent<LinkVisual>();
            linkStart.pendingLink.FollowMouse(linkStart.gameObject);
            linkStart.pendingLink.outputThatCreatedMe = linkStart;
            link.transform.SetParent(linkStart.transform, false);
        }

        public static void CompleteLink(InputVisual linkEnd)
        {
            if (linkInitiator.outputAttachedTo.ConnectTo(linkEnd.inputAttachedTo))
            {
                linkInitiator.pendingLink.FinishFollowingMouse(linkEnd.gameObject);
                linkInitiator.links.Add(linkInitiator.pendingLink);

                linkEnd.outputConnectedTo = linkInitiator;


                linkInitiator.pendingLink = null;
                linkInitiator = null;
                status = IDLE;
            }
            else
            {
                Debug.Log("linking refused");
                CancelLink();
            }
            
        }

        public static void CancelLink()
        {
            Destroy(linkInitiator.pendingLink.gameObject);
            linkInitiator = null;
            status = IDLE;
        }

        public void SetLabel(string s)
        {
            label.text = s;
            if (!stay)
            {
                label.CrossFadeAlpha(1f, 0f, true);
                label.CrossFadeAlpha(0f, 5f, true);
            }
        }

        public void Disconnect()
        {
            //disconnect from all inputs
            while(links.Count != 0)
            {
                DisconnectFrom(links[0]);
            }
        }

        public void DisconnectFrom(LinkVisual link)
        {
            outputAttachedTo.DisconnectFrom(link.inputDestination.inputAttachedTo);
            link.inputDestination.outputConnectedTo = null;
            link.inputDestination.inputAttachedTo.outputConnectedTo = null;
            Destroy(link.gameObject);
            links.Remove(link);
        }

        public static Color GetColorForType(Type t)
        {
            return InputVisual.GetColorForType(t);
        }
    }
}


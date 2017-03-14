using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VisualEditor.BackEnd;
using System.Text;

namespace VisualEditor.Visuals
{
    public class InputVisual : MonoBehaviour
    {
        public RectTransform rectTransform;
        public BackEnd.Input inputAttachedTo;//this has to be set on creation of this Gameoject
        public OutputVisual outputConnectedTo;//the output this input has been connected to, if not null
        public NodeVisual host;//set on creating this gameobject
        public Text label;//text displayed next to the input on hover

        private Image img;
        public bool stay = false;

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
            label.alignment = TextAnchor.MiddleRight;
            label.fontSize = 17;
            label.rectTransform.anchoredPosition = new Vector2(-93f, 0f);
            label.raycastTarget = false;

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
        }

        private void PointerEnter(BaseEventData eventData)
        {
            if (!stay)
                ShowName();
        }

        public void ShowName()
        {
            label.CrossFadeAlpha(1f, 0.3f, true);
        }

        public void HideName()
        {
            if(!stay)
                label.CrossFadeAlpha(0f, 0.2f, true);
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
            HideName();
        }

        private void OnClick(BaseEventData arg0)
        {
            PointerEventData ev = arg0 as PointerEventData;
            if (ev == null)
                return;
            if(ev.button == PointerEventData.InputButton.Left)
            {
                
                if (OutputVisual.status == OutputVisual.LINKING)
                {
                    //linking
                    if(OutputVisual.linkInitiator.host != host)
                    {
                        Disconnect();
                        OutputVisual.CompleteLink(this);
                    }
                }
                else
                {
                    //not linking
                    if (ev.clickCount == 2)
                    {
                        Disconnect();
                    }
                }
            }
            else if(ev.button == PointerEventData.InputButton.Right)
            {
                stay = !stay;
            }
            else if(ev.button == PointerEventData.InputButton.Middle)
            {
                List<Type> allowed = inputAttachedTo.GetAllowedDataTypes();
                if(allowed.Count != 0)
                {
                    StringBuilder builder = new StringBuilder("Allowed:\n");
                    for(int i = 0; i < allowed.Count; i++)
                    {
                        string cleanName = TabMenu.CleanClassName(allowed[i].ToString());
                        builder.Append(cleanName);
                        if (i != allowed.Count - 1)
                            builder.Append(", ");
                    }
                    MouseToast.MakeToastFixed(2f, builder.ToString(), UnityEngine.Input.mousePosition);
                }
            }

            
        }

        public void SetType(Type type)
        {
            //change the color/shape of the input base on the type given
            img.color = GetColorForType(type);
        }

        public void UpdateType()
        {
            //change color based on the allowed data types, if the data types only contains 1 type call SetType on it
            List<Type> types =  inputAttachedTo.GetAllowedDataTypes();

            if (types.Count == 1)
            {
                SetType(types[0]);
                return;
            }
            float r, g, b;
            r = g = b = 0f;
            for(int i = 0; i < types.Count; i++)
            {
                Color col = GetColorForType(types[i]);
                r += col.r;
                g += col.g;
                b += col.b;
            }
            r /= types.Count;
            g /= types.Count;
            b /= types.Count;

            img.color = new Color(r, g, b, 1f);
        }

        public void Disconnect()
        {
            //check if necessary and if so disconnect
            if(outputConnectedTo != null)
            {
                outputConnectedTo.DisconnectFrom(outputConnectedTo.links.Find(
                (LinkVisual l) => l.inputDestination == this
                ));
            }
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

        public static Color GetColorForType(Type t)
        {
            if(t == typeof(object))
            {
                return Color.white;
            }
            else if (t == typeof(float))
            {
                return new Color(3 / 255f, 162 / 255f, 184 / 255f, 1f);
            }
            else if (t == typeof(bool))
            {
                return new Color(217 / 255f, 197 / 255f, 180 / 255f, 1f);
            }
            else if (t == typeof(Vector2))
            {
                return new Color(191 / 255f, 160 / 255f, 92 / 255f, 1f);
            }
            else if (t == typeof(Vector3))
            {
                return new Color(127 / 255f, 106 / 255f, 61 / 255f, 1f);
            }
            else if (t == typeof(GameObject))
            {
                return new Color(22 / 255f, 6 / 255f, 131 / 255f, 1f);
            }
            else
            {
                return Color.magenta;
            }
        }
    }
}


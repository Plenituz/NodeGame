using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class WaitNodeVisual : NodeVisual
    {
        InputField inField;
        Text tex;
        WaitNode mNode;
        public bool checkMannualy;

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (WaitNode)node;

            inField = (Instantiate(Resources.Load<GameObject>("InputField")) as GameObject).GetComponent<InputField>();
            inField.contentType = InputField.ContentType.DecimalNumber;
            inField.characterLimit = 3;
            inField.textComponent.alignment = TextAnchor.MiddleCenter;
            inField.onValueChanged.AddListener(OnChange);
            ((Text)inField.placeholder).text = "time";

            RectTransform rect = inField.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(45f, 30f);
            rect.SetParent(transform, false);

            GameObject t = Instantiate(Resources.Load<GameObject>("BaseText")) as GameObject;
            t.transform.SetParent(transform, false);
            t.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -26.5f);
            tex = t.GetComponent<Text>();
            tex.text = "(sec)";
            tex.fontSize = 17;
        }

        void OnChange(string s)
        {
            float value = 0f;
            if (!s.Equals("") && !s.Equals("-") && !s.Equals("."))
            {
                try
                {
                    value = float.Parse(s);
                }
                catch (FormatException)
                {
                    Debug.LogError("Could not cast from string to float (WaitNode)");
                }
            }
            mNode.SetWaitTime(value);
        }

        internal override string GetDisplayName()
        {
            return "Wait";
        }

        internal override float GetHeight()
        {
            return 100f;
        }

        internal override Node GetNode()
        {
            return new WaitNode();
        }

        internal override float GetWidth()
        {
            return 100f;
        }

        internal override void PersonnalizeSetup()
        {
            checkMannualy = mNode.checkManually;
            if (mNode.checkManually)
            {
                inField.gameObject.SetActive(true);
                tex.gameObject.SetActive(true);
                OnChange(inField.text);
            }
            else
            {
                inField.gameObject.SetActive(false);
                tex.gameObject.SetActive(false);
            }
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "anything";
                case 1:
                    return "wait time";
            }
            return "null";
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "input value after a delay";
        }

        protected override void DoInitFromNode()
        {
            if (inField.gameObject.activeSelf)
                inField.text = mNode.waitTime.ToString();
        }
    }
}

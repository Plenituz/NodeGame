using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class GroupOutputNodeVisual : NodeVisual
    {
        InputField field;

        protected override void DoInitFromNode()
        {
            field.text = ((GroupOutputNode)node).outName;
        }

        internal override void BeginPersonnalizeSetup()
        {
            GameObject fieldObj = Instantiate(Resources.Load<GameObject>("InputField")) as GameObject;
            fieldObj.transform.SetParent(transform, false);
            RectTransform fieldTr = fieldObj.GetComponent<RectTransform>();
            fieldTr.anchoredPosition = new Vector2(0f, -56f);

            field = fieldObj.GetComponent<InputField>();
            ((Text)field.placeholder).text = "output name";
            field.text = "output";
            field.characterLimit = 20;
            field.textComponent.alignment = TextAnchor.MiddleCenter;
            field.onValueChanged.AddListener(OnNameChanged);
        }

        void OnNameChanged(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                ((GroupOutputNode)node).outName = s;
            }
            
        }

        internal override string GetDisplayName()
        {
            return "Group Output";
        }

        internal override float GetHeight()
        {
            return 75f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "group output";
        }

        internal override Node GetNode()
        {
            return new GroupOutputNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "null";
        }

        internal override float GetWidth()
        {
            return 75f;
        }

        internal override void PersonnalizeSetup()
        {
        }
    }
}

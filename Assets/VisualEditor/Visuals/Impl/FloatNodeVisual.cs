using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;
using UnityEngine;
using UnityEngine.UI;

namespace VisualEditor.Visuals.Impl
{
    public class FloatNodeVisual : NodeVisual
    {
        InputField field;
        FloatNode mNode;
        float widthBox = 0f;

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (FloatNode)node;

            GameObject g = Instantiate(Resources.Load<GameObject>("InputField")) as GameObject;
            field = g.GetComponent<InputField>();
            field.transform.SetParent(transform, false);
            field.onValueChanged.AddListener(OnValueChange);
            field.onEndEdit.AddListener(OnValueChange);
            field.characterLimit = 8;
            field.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 30f);
            ((Text)field.placeholder).text = "";
            Utils.SetupFieldAsDecimal(field);
        }

        void OnValueChange(string s)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            widthBox = LayoutUtility.GetPreferredWidth(field.textComponent.rectTransform);
            field.GetComponent<RectTransform>().sizeDelta = new Vector2(30f + widthBox, 30f);

            rectTransform.sizeDelta = new Vector2(GetWidth(), GetHeight());
            PlaceOutputs();

            float val = 0f;
            if (!field.text.Equals("") && !field.text.Equals("-"))
            {
                try
                {
                    val = float.Parse(field.text);
                }
                catch (FormatException)
                {
                    Debug.LogError("Could not cast input into a float");
                }
            }
            mNode.SetFloat(val, field.text);
        }

        internal override string GetDisplayName()
        {
            return "Decimal";
        }

        internal override float GetHeight()
        {
            return 50;
        }

        internal override Node GetNode()
        {
            return new FloatNode();
        }

        internal override float GetWidth()
        {
            return 50 + widthBox;
        }

        internal override void PersonnalizeSetup()
        {
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "null";
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "value";
        }

        protected override void DoInitFromNode()
        {
            field.text = mNode.strValue;

            for (int i = 0; i < field.text.Length; i++)
            {
                UpdateSize();
            }
        }
    }
}

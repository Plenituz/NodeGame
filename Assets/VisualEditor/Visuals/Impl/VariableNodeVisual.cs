using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class VariableNodeVisual : NodeVisual
    {
        InputField field;
        VariableNode mNode;
        SplatMenu splat;
        float widthBox = 0f;

        protected override void DoInitFromNode()
        {
            string name = mNode.varName;
            field.text = name;
            mNode.SetVariableName(name);
        }

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (VariableNode)node;
            mNode.onTypeChanged = OnTypeChange;

            GameObject fieldHolder = Instantiate(Resources.Load<GameObject>("InputField")) as GameObject;
            fieldHolder.transform.SetParent(transform, false);
            field = fieldHolder.GetComponent<InputField>();
            field.characterLimit = 10;
            field.textComponent.alignment = TextAnchor.MiddleCenter;
            ((Text)field.placeholder).text = "name";
            field.onEndEdit.AddListener(OnFinishText);
            field.onValueChanged.AddListener(OnTextChange);

            RectTransform rect = field.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60f, 30f);
            rect.anchoredPosition = new Vector2(0f, 30f);

            splat = SplatMenu.Create(this, new string[] { "vec2", "vec3", "obj", "flt", "bool" }, OnSelectSplat, new Vector2(55f, 55f), 75f, false);
            splat.gameObject.AddComponent<RectTransform>().anchoredPosition = new Vector2(0f, -20f);
            if (doStart)
            {
                StartCoroutine(StartSelect());
            }
        }

        IEnumerator StartSelect()
        {
            yield return new WaitForEndOfFrame();
            splat.Select(splat.GetItemForText(GroupInputNodeVisual.typeToStr[mNode.varType]));
        }

        void OnTypeChange(Type t)
        {
            splat.Select(splat.GetItemForText(GroupInputNodeVisual.typeToStr[t]), true);
        }

        void OnSelectSplat(string select)
        {
            mNode.SetVariableType(GroupInputNodeVisual.strToType[select]);
        }

        void OnFinishText(string str)
        {
            mNode.SetVariableName(str);
            OnTextChange(str);
        }

        void OnTextChange(string str)
        {
            float width = LayoutUtility.GetPreferredWidth(field.textComponent.rectTransform);
            if (width + 30f < 60f)
                widthBox = 0f;
            else
                widthBox = width - 30f;
            field.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(30f + width, 60f), 30f);
            rectTransform.sizeDelta = new Vector2(GetWidth(), GetHeight());
            PlaceInputs();
            PlaceOutputs();
        }

        internal override string GetDisplayName()
        {
            return "Variable";
        }

        internal override float GetHeight()
        {
            return 115f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "value to save";
        }

        internal override Node GetNode()
        {
            return new VariableNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "saved value";
        }

        internal override float GetWidth()
        {
            return 90f + widthBox;
        }

        internal override void PersonnalizeSetup()
        {
        }
    }
}

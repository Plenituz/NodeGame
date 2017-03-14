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
    class ComplexMathNodeVisual : NodeVisual
    {
        private ComplexMathNode mNode;
        InputField field;
        RectTransform fieldTrans;
        Vector2 oldSize;

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (ComplexMathNode)node;
            oldSize = new Vector2(GetWidth(), GetHeight());
            GameObject fieldHolder = Instantiate(Resources.Load<GameObject>("InputField")) as GameObject;
            field = fieldHolder.GetComponent<InputField>();
            fieldTrans = field.GetComponent<RectTransform>();
            fieldTrans.SetParent(transform, false);
            fieldTrans.sizeDelta = new Vector2(150f, 150f);
            field.onValueChanged.AddListener(OnExprChange);
            field.lineType = InputField.LineType.MultiLineSubmit;
            ((Text)field.placeholder).text = "type \"in1\", \"in2\" etc to get the data from the inputs";
        }

        void OnExprChange(string expr)
        {
            mNode.SetExpression(expr);
        }

        internal override string GetDisplayName()
        {
            return "Math";
        }

        internal override float GetHeight()
        {
            return Mathf.Max(200f, node.inputs.Count * 25f + 10f);
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "in" + (inputIndex + 1);
        }

        internal override Node GetNode()
        {
            return new ComplexMathNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "value";
        }

        internal override float GetWidth()
        {
            return 200f;
        }

        internal override void PersonnalizeSetup()
        {
            if (mNode.evaluable)
            {
                if (background.color == Color.gray)
                    background.color = Color.white;
            }
            else
            {
                if (background.color == Color.white)
                    background.color = Color.gray;
            }

            if (oldSize.y != GetHeight())
            {
                oldSize = new Vector2(GetWidth(), GetHeight());
                rectTransform.sizeDelta = oldSize;
                ((RectTransform)rectTransform.FindChild("display text")).anchoredPosition = new Vector2(0f, GetHeight() / 2 + 20f);
                fieldTrans.sizeDelta = new Vector2(150f, GetHeight() - 50f);
                PlaceInputs();
                PlaceOutputs();
                PlaceCommentText();
            }
        }

        protected override void DoInitFromNode()
        {
            field.text = mNode.GetExpression();
            mNode.SetExpression(mNode.GetExpression());
        }
    }
}

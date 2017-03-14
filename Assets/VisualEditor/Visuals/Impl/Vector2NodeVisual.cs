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
    public class Vector2NodeVisual : NodeVisual
    {
        InputField inX, inY;
        float widthBox = 0f;
        Vector2Node mNode;

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (Vector2Node)node;

            GameObject prefab = Resources.Load<GameObject>("InputField");
            inX = (Instantiate(prefab) as GameObject).GetComponent<InputField>();
            inY = (Instantiate(prefab) as GameObject).GetComponent<InputField>();
            inX.transform.SetParent(transform, false);
            inY.transform.SetParent(transform, false);
            inX.onValueChanged.AddListener(OnTextChanged);
            inY.onValueChanged.AddListener(OnTextChanged);
            inX.characterLimit = 8;
            inY.characterLimit = 8;
            inX.contentType = InputField.ContentType.DecimalNumber;
            inY.contentType = InputField.ContentType.DecimalNumber;
            inX.textComponent.alignment = TextAnchor.MiddleCenter;
            inY.textComponent.alignment = TextAnchor.MiddleCenter;
            inX.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            inY.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            inY.onEndEdit.AddListener(OnEnd);
            inX.onEndEdit.AddListener(OnEndX);
            ((Text)inX.placeholder).text = "x";
            ((Text)inY.placeholder).text = "y";

            RectTransform rx = inX.GetComponent<RectTransform>();
            rx.sizeDelta = new Vector2(30f, 30f);
            rx.anchoredPosition = new Vector2(0f, 20f);

            RectTransform ry = inY.GetComponent<RectTransform>();
            ry.sizeDelta = new Vector2(30f, 30f);
            ry.anchoredPosition = new Vector2(0f, -20f);
            
        }

        void OnTextChanged(string t)
        {
            UpdateBoxAndInputs();
        }

        void OnEndX(string s)
        {
            inY.ActivateInputField();
            inY.Select();
            UpdateBoxAndInputs();
        }

        void OnEnd(string s)
        {
            UpdateBoxAndInputs();
        }

        void UpdateBoxAndInputs()
        {
            float widthX = LayoutUtility.GetPreferredWidth(inX.textComponent.rectTransform);
            float widthY = LayoutUtility.GetPreferredWidth(inY.textComponent.rectTransform);
            widthBox = Mathf.Max(widthX, widthY);
            inX.GetComponent<RectTransform>().sizeDelta = new Vector2(30f + widthBox, 30f);
            inY.GetComponent<RectTransform>().sizeDelta = new Vector2(30f + widthBox, 30f);

            rectTransform.sizeDelta = new Vector2(GetWidth(), GetHeight());
            PlaceOutputs();
            PlaceInputs();

            float x, y;
            x = y = 0f;
            if (inX.gameObject.activeSelf)
            {
                if (!inX.text.Equals("") && !inX.text.Equals("-") && !inX.Equals("."))
                {
                    try
                    {
                        x = float.Parse(inX.text);
                    }
                    catch (FormatException)
                    {
                        Debug.LogError("Could not cast inputs (x) into a float, you ninja");
                    }
                }
                mNode.SetVectorX(x, inX.text);
            }

            if (inY.gameObject.activeSelf)
            {
                if (!inY.text.Equals("") && !inY.text.Equals("-") && !inY.Equals("."))
                {
                    try
                    {
                        y = float.Parse(inY.text);
                    }
                    catch
                    {
                        Debug.LogError("Could not cast inputs (y) into a float, you ninja");
                    }
                }
                mNode.SetVectorY(y, inY.text);
            }
        }

        internal override string GetDisplayName()
        {
            return "Vector2";
        }

        internal override float GetHeight()
        {
            return 100f;
        }

        internal override Node GetNode()
        {
            return new Vector2Node();
        }

        internal override float GetWidth()
        {
            return 70f + widthBox;
        }

        internal override void PersonnalizeSetup()
        {
            if(node.inputs[0].GetDataType() == null)
            {
                //show inputX
                if (!inX.gameObject.activeSelf)
                {
                    inX.gameObject.SetActive(true);
                    UpdateBoxAndInputs();
                }
            }
            else
            {
                //hide inputX
                if (inX.gameObject.activeSelf)
                    inX.gameObject.SetActive(false);
            }

            if (node.inputs[1].GetDataType() == null)
            {
                //show inputY
                if (!inY.gameObject.activeSelf)
                {
                    inY.gameObject.SetActive(true);
                    UpdateBoxAndInputs();
                }
            }
            else
            {
                //hide inputY
                if (inY.gameObject.activeSelf)
                    inY.gameObject.SetActive(false);
            }
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "x";
                case 1:
                    return "y";
            }
            return "null";
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "value";
        }

        protected override void DoInitFromNode()
        {
            string vx = mNode.strX;
            string vy = mNode.strY;
            if (inX.gameObject.activeSelf)
                inX.text = vx;
            if (inY.gameObject.activeSelf)
                inY.text = vy;
            
            for(int i = 0; i < Mathf.Max(vx.Length, vy.Length); i++)
            {
                UpdateBoxAndInputs();
            }
        }
    }
}

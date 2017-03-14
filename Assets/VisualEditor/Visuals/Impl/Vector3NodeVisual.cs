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
    public class Vector3NodeVisual : NodeVisual
    {
        float widthBox = 0f;
        Vector3Node mNode;
        InputField inX, inY, inZ;

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (Vector3Node)node;

            GameObject prefab = Resources.Load<GameObject>("InputField");
            inX = (Instantiate(prefab) as GameObject).GetComponent<InputField>();
            inY = (Instantiate(prefab) as GameObject).GetComponent<InputField>();
            inZ = (Instantiate(prefab) as GameObject).GetComponent<InputField>();

            inX.transform.SetParent(transform, false);
            inY.transform.SetParent(transform, false);
            inZ.transform.SetParent(transform, false);

            inX.onValueChanged.AddListener(OnEnd);
            inY.onValueChanged.AddListener(OnEnd);
            inZ.onValueChanged.AddListener(OnEnd);

            inX.characterLimit = 8;
            inY.characterLimit = 8;
            inZ.characterLimit = 8;

            inX.contentType = InputField.ContentType.DecimalNumber;
            inY.contentType = InputField.ContentType.DecimalNumber;
            inZ.contentType = InputField.ContentType.DecimalNumber;

            inX.textComponent.alignment = TextAnchor.MiddleCenter;
            inY.textComponent.alignment = TextAnchor.MiddleCenter;
            inZ.textComponent.alignment = TextAnchor.MiddleCenter;

            inX.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            inY.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            inZ.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;

            inY.onEndEdit.AddListener(OnEndY);
            inX.onEndEdit.AddListener(OnEndX);
            inZ.onEndEdit.AddListener(OnEnd);

            ((Text)inX.placeholder).text = "x";
            ((Text)inY.placeholder).text = "y";
            ((Text)inZ.placeholder).text = "z";

            RectTransform rx = inX.GetComponent<RectTransform>();
            rx.sizeDelta = new Vector2(30f, 30f);
            rx.anchoredPosition = new Vector2(0f, 40f);

            RectTransform ry = inY.GetComponent<RectTransform>();
            ry.sizeDelta = new Vector2(30f, 30f);
            ry.anchoredPosition = new Vector2(0f, 0f);

            RectTransform rz = inZ.GetComponent<RectTransform>();
            rz.sizeDelta = new Vector2(30f, 30f);
            rz.anchoredPosition = new Vector2(0f, -40f);
        }

        private void OnEnd(string arg0)
        {
            UpdateBoxAndInputs();
        }

        private void OnEndX(string arg0)
        {

            if (UnityEngine.Input.GetKey(KeyCode.Return) || UnityEngine.Input.GetKey(KeyCode.KeypadEnter))
            {
                inY.ActivateInputField();
                inY.Select();
            }

            UpdateBoxAndInputs();
        }

        private void OnEndY(string arg0)
        {
            if (UnityEngine.Input.GetKey(KeyCode.Return) || UnityEngine.Input.GetKey(KeyCode.KeypadEnter))
            {
                inZ.ActivateInputField();
                inZ.Select();
            }
            UpdateBoxAndInputs();
        }

        void UpdateBoxAndInputs()
        {
            float widthX = LayoutUtility.GetPreferredWidth(inX.textComponent.rectTransform);
            float widthY = LayoutUtility.GetPreferredWidth(inY.textComponent.rectTransform);
            float widthZ = LayoutUtility.GetPreferredWidth(inZ.textComponent.rectTransform);
            widthBox = Mathf.Max(Mathf.Max(widthX, widthY), widthZ);
            inX.GetComponent<RectTransform>().sizeDelta = new Vector2(30f + widthBox, 30f);
            inY.GetComponent<RectTransform>().sizeDelta = new Vector2(30f + widthBox, 30f);
            inZ.GetComponent<RectTransform>().sizeDelta = new Vector2(30f + widthBox, 30f);

            rectTransform.sizeDelta = new Vector2(GetWidth(), GetHeight());
            PlaceOutputs();
            PlaceInputs();

            float x, y, z;
            x = y = z = 0f;
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

            if (inZ.gameObject.activeSelf){
                if (!inZ.text.Equals("") && !inZ.text.Equals("-") && !inZ.Equals("."))
                {
                    try
                    {
                        z = float.Parse(inZ.text);
                    }
                    catch
                    {
                        Debug.LogError("Could not cast inputs (z) into a float, you ninja");
                    }
                }
                mNode.SetVectorZ(z, inZ.text);
            }
        }

        internal override string GetDisplayName()
        {
            return "Vector3";
        }

        internal override float GetHeight()
        {
            return 150f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "x";
                case 1:
                    return "y";
                case 2:
                    return "z";
            }
            return "null";
        }

        internal override Node GetNode()
        {
            return new Vector3Node();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "value";
        }

        internal override float GetWidth()
        {
            return 70f + widthBox;
        }

        internal override void PersonnalizeSetup()
        {
            if (node.inputs[0].GetDataType() == null)
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

            if(node.inputs[2].GetDataType() == null)
            {
                if (!inZ.gameObject.activeSelf)
                {
                    inZ.gameObject.SetActive(true);
                    UpdateBoxAndInputs();
                }
            }
            else
            {
                if (inZ.gameObject.activeSelf)
                    inZ.gameObject.SetActive(false);
            }
        }

        protected override void DoInitFromNode()
        {
            string vx = mNode.strX;
            string vy = mNode.strY;
            string vz = mNode.strZ;

            if (inX.gameObject.activeSelf)
                inX.text = vx;
            if (inY.gameObject.activeSelf)
                inY.text = vy;
            if (inZ.gameObject.activeSelf)
                inZ.text = vz;

            for (int i = 0; i < Mathf.Max(Mathf.Max(vx.Length, vy.Length), vz.Length); i++)
            {
                UpdateBoxAndInputs();
            }
        }
    }
}

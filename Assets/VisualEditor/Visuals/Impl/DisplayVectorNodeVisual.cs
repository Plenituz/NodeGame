using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;
using UnityEngine.UI;
using UnityEngine;

namespace VisualEditor.Visuals.Impl
{
    class DisplayVectorNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
            GameObject holder = Instantiate(Resources.Load<GameObject>("InputField")) as GameObject;
            holder.transform.SetParent(transform, false);
            InputField field = holder.GetComponent<InputField>();
            field.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
            ((Text)field.placeholder).text = "R";
        }

        internal override string GetDisplayName()
        {
            return "Display Vector";
        }

        internal override float GetHeight()
        {
            return 75f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "vector";
                case 1:
                    return "start pos";
            }
            return "null";
        }

        internal override Node GetNode()
        {
            return new DisplayVectorNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "null";
        }

        internal override float GetWidth()
        {
            return 50f;
        }

        internal override void PersonnalizeSetup()
        {
            
        }
    }
}

using UnityEngine;
using System.Collections;
using VisualEditor.Visuals;
using System;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class BoolNodeVisual : NodeVisual
    {
        BoolNode mNode;
        SplatMenu menu;
        internal override void BeginPersonnalizeSetup()
        {
            mNode = (BoolNode)node;
            menu = SplatMenu.Create(this, new string[] { "True", "False" }, OnSelect, new Vector2(75f, 50f), 100f, doStart);
        }

        void OnSelect(string select)
        {
            mNode.SetOutputValue(select.Equals("True"));
        }

        internal override string GetDisplayName()
        {
            return "Boolean";
        }

        internal override float GetHeight()
        {
            return 75f;
        }

        internal override Node GetNode()
        {
            return new BoolNode();
        }

        internal override float GetWidth()
        {
            return 100f;
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
            menu.Select(menu.GetItemForText(mNode.output ? "True" : "False"));
        }
    }
}

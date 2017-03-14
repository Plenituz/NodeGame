using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;
using UnityEngine;

namespace VisualEditor.Visuals.Impl
{
    public class AddForceNodeVisual : NodeVisual
    {
        AddForceNode mNode;

        SplatMenu menu;
        internal override void BeginPersonnalizeSetup()
        {
            mNode = (AddForceNode)node;
            menu = SplatMenu.Create(this, new string[] { "Pulse", "Push" }, OnSelect, new Vector2(80f, 80f), 100f, doStart);
        }

        void OnSelect(string s)
        {
            mNode.SetForceMode(s.Equals("Pulse") ? ForceMode2D.Impulse : ForceMode2D.Force);
        }

        internal override string GetDisplayName()
        {
            return "Add Force";
        }

        internal override float GetHeight()
        {
            return 120f;
        }

        internal override Node GetNode()
        {
            return new AddForceNode();
        }

        internal override float GetWidth()
        {
            return 120f;
        }

        internal override void PersonnalizeSetup()
        {
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "target";
                case 1:
                    return "force vector";
            }
            return "null";
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "null";
        }

        protected override void DoInitFromNode()
        {
            menu.Select(menu.GetItemForText(mNode.forceModestr.Replace("Impulse", "Pulse").Replace("Force", "Push")));
        }
    }
}

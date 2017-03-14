using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class MouseDataNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
        }

        internal override string GetDisplayName()
        {
            return "Mouse data";
        }

        internal override float GetHeight()
        {
            return 90f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "null";
        }

        internal override Node GetNode()
        {
            return new MouseDataNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            switch (outputIndex)
            {
                case 0:
                    return "position (world space)";
                case 1:
                    return "left click";
                case 2:
                    return "right click";
            }
            return "null";
        }

        internal override float GetWidth()
        {
            return 30f;
        }

        internal override void PersonnalizeSetup()
        {
        }
    }
}

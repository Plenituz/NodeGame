using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals
{
    public class NegateNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
        }

        internal override string GetDisplayName()
        {
            return "Negate";
        }

        internal override float GetHeight()
        {
            return 40f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "input";
        }

        internal override Node GetNode()
        {
            return new NegateNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "input *-1";
        }

        internal override float GetWidth()
        {
            return 35f;
        }

        internal override void PersonnalizeSetup()
        {
        }
    }
}

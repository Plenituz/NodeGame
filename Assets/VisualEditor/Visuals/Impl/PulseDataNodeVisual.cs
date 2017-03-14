using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    class PulseDataNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
        }

        internal override string GetDisplayName()
        {
            return "Pulse";
        }

        internal override float GetHeight()
        {
            return 40f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "in";
        }

        internal override Node GetNode()
        {
            return new PulseDataNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "out as pulse";
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

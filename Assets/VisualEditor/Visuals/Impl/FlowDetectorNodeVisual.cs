using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class FlowDetectorNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
        }

        internal override string GetDisplayName()
        {
            return "Flow detect.";
        }

        internal override float GetHeight()
        {
            return 50f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "input";
        }

        internal override Node GetNode()
        {
            return new FlowDetectorNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "flow";
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

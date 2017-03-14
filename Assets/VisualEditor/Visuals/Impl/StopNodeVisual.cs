using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class StopNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
            
        }

        internal override void BeginPersonnalizeSetup()
        {
            
        }

        internal override string GetDisplayName()
        {
            return "Stop";
        }

        internal override float GetHeight()
        {
            return 40f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "stop ?";
        }

        internal override Node GetNode()
        {
            return new StopNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "null";
        }

        internal override float GetWidth()
        {
            return 40f;
        }

        internal override void PersonnalizeSetup()
        {
        }
    }
}

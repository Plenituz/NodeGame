using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class DebugNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
            
        }

        internal override string GetDisplayName()
        {
            return "Debug";
        }

        internal override float GetHeight()
        {
            return 50f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "object to log";
        }

        internal override Node GetNode()
        {
            return new DebugNode();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    class TimeNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
            
        }

        internal override void BeginPersonnalizeSetup()
        {
            
        }

        internal override string GetDisplayName()
        {
            return "Time";
        }

        internal override float GetHeight()
        {
            return 50f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "null";
        }

        internal override Node GetNode()
        {
            return new TimeNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "time";
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

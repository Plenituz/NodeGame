using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class GoIfTrueNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {

        }

        internal override string GetDisplayName()
        {
            return "Validate";
        }

        internal override float GetHeight()
        {
            return 70f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "input";
                case 1:
                    return "can signal pass ?";
            }
            return "null";
        }

        internal override Node GetNode()
        {
            return new GoIfTrueNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "data from input";
        }

        internal override float GetWidth()
        {
            return 90f;
        }

        internal override void PersonnalizeSetup()
        {
            
        }
    }
}

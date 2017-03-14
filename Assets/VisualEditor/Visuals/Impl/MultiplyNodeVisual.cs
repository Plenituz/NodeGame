using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class MultiplyNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
        }

        internal override string GetDisplayName()
        {
            return "Multiply";
        }

        internal override float GetHeight()
        {
            return 65f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "value to multiply";
                case 1:
                    return "multiplicator";
            }
            return "null";
        }

        internal override Node GetNode()
        {
            return new MultiplyNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "values multiplied";
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

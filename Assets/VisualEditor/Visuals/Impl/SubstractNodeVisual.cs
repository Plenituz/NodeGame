using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class SubstractNodeVisual : AddNodeVisual
    {
        internal override string GetDisplayName()
        {
            return "Substract";
        }

        internal override Node GetNode()
        {
            return new SubtractNode();
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "this minus";
                case 1:
                    return "this";
            }
            return "null";
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "result";
        }
    }
}

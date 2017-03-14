using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class VectorComponentsNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
        }

        internal override string GetDisplayName()
        {
            return "Vector Components";
        }

        internal override float GetHeight()
        {
            return 110f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "vector";
        }

        internal override Node GetNode()
        {
            return new VectorComponentsNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            switch (outputIndex)
            {
                case 0:
                    return "x";
                case 1:
                    return "y";
            }
            if(node.outputs.Count == 4)
            {
                //vector3
                switch (outputIndex)
                {
                    case 2:
                        return "z";
                    case 3:
                        return "magnitude";
                }
            }
            else
            {
                //vector2
                switch (outputIndex)
                {
                    case 2:
                        return "magnitude";
                }
            }
            return "null";
        }

        internal override float GetWidth()
        {
            return 80f;
        }

        internal override void PersonnalizeSetup()
        {

        }
    }
}

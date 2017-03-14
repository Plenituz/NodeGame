using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class EntityDataNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
            
        }

        internal override string GetDisplayName()
        {
            return "Object data";
        }

        internal override float GetHeight()
        {
            return 100f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "object";
        }

        internal override Node GetNode()
        {
            return new EntityDataNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            switch (outputIndex)
            {
                case 0:
                    return "position";
                case 1:
                    return "rotation";
                case 2:
                    return "velocity";
            }
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

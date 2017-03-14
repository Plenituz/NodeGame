using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class DealDamageNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {

        }

        internal override string GetDisplayName()
        {
            return "Damage";
        }

        internal override float GetHeight()
        {
            return 100f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "target";
                case 1:
                    return "power";
            }
            return "null";
        }

        internal override Node GetNode()
        {
            return new DealDamageNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "null";
        }

        internal override float GetWidth()
        {
            return 100f;
        }

        internal override void PersonnalizeSetup()
        {
            
        }
    }
}

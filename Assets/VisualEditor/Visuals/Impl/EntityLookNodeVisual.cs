using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class EntityLookNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
        }

        internal override void BeginPersonnalizeSetup()
        {
            
        }

        internal override string GetDisplayName()
        {
            return "Look";
        }

        internal override float GetHeight()
        {
            return 40f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "entity to study";
        }

        internal override Node GetNode()
        {
            return new EntityLookNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "direction the entity is looking";
        }

        internal override float GetWidth()
        {
            return 35f;
        }

        internal override void PersonnalizeSetup()
        {
        }
    }
}

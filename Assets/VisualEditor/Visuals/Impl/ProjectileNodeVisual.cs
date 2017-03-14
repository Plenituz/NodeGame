using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class ProjectileNodeVisual : NodeVisual
    {
        protected override void DoInitFromNode()
        {
            
        }

        internal override void BeginPersonnalizeSetup()
        {
            
        }

        internal override string GetDisplayName()
        {
            return "Projectile";
        }

        internal override float GetHeight()
        {
            return 60f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "direction";
                case 1:
                    return "speed";
            }
            return "null";
        }

        internal override Node GetNode()
        {
            return new ProjectileNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            switch (outputIndex)
            {
                case 0:
                    return "projectile";
                case 1:
                    return "object hit";
            }
            return "null";
        }

        internal override float GetWidth()
        {
            return 60f;
        }

        internal override void PersonnalizeSetup()
        {
            
        }
    }
}

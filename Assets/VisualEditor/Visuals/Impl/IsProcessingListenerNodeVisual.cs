using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class IsProcessingListenerNodeVisual : ListenerBaseNodeVisual
    {

        internal override string GetDisplayName()
        {
            return "Processing Listener";
        }

        internal override float GetHeight()
        {
            return 100f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "null";
        }

        internal override Node GetNode()
        {
            return new IsProcessingListenerNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "is processing?";
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

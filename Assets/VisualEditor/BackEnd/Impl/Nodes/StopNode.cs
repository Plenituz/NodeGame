using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class StopNode : Node
    {
        public override void BeginSetup()
        {
            inputs.Add(new BoolInput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(StopNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Stop the spell if the input is true";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(bool) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Stop", "end" };
        }

        protected override void DoReset()
        {
            
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            bool val = (bool)inputs[0].GetData();
            if (val)
            {
                StopSpell();
            }
            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {
            
        }
    }
}

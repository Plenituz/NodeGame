using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    public class FlowDetectorNode : Node
    {
        private bool flow = false;

        public override void BeginSetup()
        {
            inputs.Add(new AnyTypeInput(this));
            outputs.Add(new BoolOutput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(FlowDetectorNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Outputs true if there is data coming in the input, false otherwise";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(object) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(bool) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Flow detector" };
        }

        protected override void DoReset()
        {
            
        }

        protected override void OnUpdate()
        {
            if (!flow)
            {
                outputs[0].SetData(false);
                ProcessAllOutputs();
            }
            else
            {
                flow = false;
            }
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            flow = true;
            outputs[0].SetData(true);
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
        }
    }
}

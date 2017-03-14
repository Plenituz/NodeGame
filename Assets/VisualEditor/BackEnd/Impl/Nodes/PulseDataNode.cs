using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    class PulseDataNode : Node
    {
        AnyTypeInput input;
        AnyTypeOutput output;
        [NonSerialized]
        bool hasFlow = false;
        [NonSerialized]
        bool oldHasFlow = false;

        public override void BeginSetup()
        {
            input = new AnyTypeInput(this);
            inputs.Add(input);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(PulseDataNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(object) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(object) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Pulse data", "break flow", "one shot"};
        }

        protected override void DoReset()
        {
        }

        protected override void OnUpdate()
        {
            if(hasFlow && !oldHasFlow)
            {
                //if there is a new data flow
                output.SetData(input.GetData());
                ProcessAllOutputs();
            }
            oldHasFlow = hasFlow;
            hasFlow = false;
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            //transform the constant data flow in input into a pulse data, 
            //basically outputs the first value coming in and doesn't ouput anything until the data in stops 
            //then reouput a value when the data in flows back in
            hasFlow = true;
            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {
            if (input.GetDataType() != null)
            {
                if (output == null)
                {
                    //create the output
                    output = new AnyTypeOutput(this, input.GetDataType());
                    outputs.Add(output);
                }
                else
                {
                    //update the output
                    if (input.GetDataType() != output.GetDataType())
                        output.SetDataType(input.GetDataType());
                }
            }
            else
            {
                //delete the output
                if (output != null)
                {
                    outputs.Clear();
                    output = null;
                }
            }
        }

        public override string GetDocumentation()
        {
            return "One of the most useful node. A constant flow of data is transformed into a single pulse of data. See example for a better understanding.";
        }
    }
}

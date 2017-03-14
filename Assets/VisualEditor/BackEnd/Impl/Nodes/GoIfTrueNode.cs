using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using VisualEditor.Visuals.Impl;
using System.Runtime.Serialization;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class GoIfTrueNode : Node
    {
        AnyTypeInput in1;
        BoolInput valid;
        AnyTypeOutput outp;

        public override void BeginSetup()
        {
            in1 = new AnyTypeInput(this);
            valid = new BoolInput(this);
            inputs.Add(in1);
            inputs.Add(valid);            
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(GoIfTrueNodeVisual);
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
            return new string[] { "Validate", "pass if true" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {

            bool val = (bool)valid.GetData();
            if (val)
            {
                outp.SetData(in1.GetData());
                processAllOutputs = true;
            }
            else
            {
                processAllOutputs = false;
            }
        }

        internal override void PartialSetup()
        {
            if (in1.GetDataType() != null)
            {
                if (outp == null)
                {
                    //create the output
                    outp = new AnyTypeOutput(this, in1.GetDataType());
                    outputs.Add(outp);
                }
                else
                {
                    //update the output
                    if (in1.GetDataType() != outp.GetDataType())
                        outp.SetDataType(in1.GetDataType());
                    if (outputs.Count == 0)
                        outputs.Add(outp);
                }
            }
            else
            {
                //delete the output
                 outputs.Clear();
            }
        }

        public override string GetDocumentation()
        {
            return "One of the most useful node. The data you pass in the first input is only relayed to the output if the second input receives a \"true\"";
        }
    }
}

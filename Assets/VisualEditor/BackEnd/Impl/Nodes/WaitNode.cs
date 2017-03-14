using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.Visuals.Impl;
using UnityEngine;
using System.Runtime.Serialization;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class WaitNode : Node
    {
        AnyTypeInput input;
        MultiDataInput inTime;
        AnyTypeOutput output;
        public float waitTime = 1f;

        [NonSerialized]
        bool waiting = false;
        public bool checkManually = true;
        [NonSerialized]
        float startTime;
        [NonSerialized]
        object savedDataIn;

        public void SetWaitTime(float wait)
        {
            waitTime = wait;
        }

        public override void BeginSetup()
        {
            input = new AnyTypeInput(this);
            inTime = new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) }));
            inputs.Add(input);
            inputs.Add(inTime);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(WaitNodeVisual);
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
            return new string[] {"Wait", "delay", "sampler", "resample" };
        }

        protected override void OnUpdate()
        {
            //we don't know if the data input is a constant flow so we have to update ourself
            if (checkManually && !waiting && input.hasData)
            {
                waiting = true;
                startTime = Time.time;
                savedDataIn = input.GetData();
                input.hasData = false;
            }
            if (waiting && Time.time - startTime > waitTime)
            {
                waiting = false;
                output.SetData(savedDataIn);
                ProcessAllOutputs();
            }
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if (!waiting)
            {
                //we know the input has data so we can start waiting now
                waitTime = (float)inTime.GetData();
                waiting = true;
                startTime = Time.time;
                savedDataIn = input.GetData();
                //save the data, wait some time and put the data on the output, 
                //if this is a flow a data the data is gonna be offset by some time
            }
            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {
            if(input.GetDataType() != null)
            {
                if(output == null)
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
                if(output != null)
                {
                    outputs.Clear();
                    output = null;
                }
            }
            if (inTime.GetDataType() == null)
                checkManually = true;
            else
                checkManually = false;
        }

        protected override void DoReset()
        {
            waiting = false;
        }

        public override string GetDocumentation()
        {
            return "Can also be called \"Resample\". As soon as data arrives at the first input, it is saved and outputed after a time. All the data coming in in between is ignored";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class MultiplyNode : Node
    {
        MultiDataInput in1;
        FloatInput inMult;
        AnyTypeOutput outResult;

        [NonSerialized]
        Type oldType;

        public override void BeginSetup()
        {
            in1 = new MultiDataInput(this, new List<Type>(new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) }));
            inMult = new FloatInput(this);

            inputs.Add(in1);
            inputs.Add(inMult);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(MultiplyNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Multiply", "times", "x", "*", "function", "math" };
        }

        protected override void DoReset()
        {
            
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if(in1.GetDataType() == typeof(float))
            {
                //multiply 2 float
                float v = (float)in1.GetData();
                float m = (float)inMult.GetData();
                outResult.SetData(v * m);
            }
            else if(in1.GetDataType() == typeof(Vector2))
            {
                //multiply vector2 and float
                Vector2 v = (Vector2)in1.GetData();
                float m = (float)inMult.GetData();
                outResult.SetData(v * m);
            }
            else if(in1.GetDataType() == typeof(Vector3))
            {
                //multiply vec3 and float
                Vector3 v = (Vector3)in1.GetData();
                float m = (float)inMult.GetData();
                outResult.SetData(v * m);
            }
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
            if(in1.GetDataType() == null)
            {
                //nothing plugged in
                //hide the output
                oldType = null;
                outResult = null;
                outputs.Clear();
            }
            else
            {
                if(in1.GetDataType() != oldType)
                {
                    oldType = in1.GetDataType();
                    //if the data type in input 1 changed
                    //change/create output
                    if(outResult == null)
                    {
                        //create output adapted to in type
                        outResult = new AnyTypeOutput(this, in1.GetDataType());
                        outputs.Add(outResult);
                    }
                    else
                    {
                        //update output for in type
                        outResult.SetDataType(in1.GetDataType());
                    }
                }
            }
        }

        public override string GetDocumentation()
        {
            return "Multiply the 2 inputs together. If the first input is a vector, the output is going to be a the same vector with a lenght (magnitude) multiplied by the second input.";
        }
    }
}

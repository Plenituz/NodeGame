using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class AddNode : Node
    {
        Type old1;
        Type old2;
        MultiDataInput in1, in2;
        AnyTypeOutput outp;

        public override void BeginSetup()
        {
            in1 = new MultiDataInput(this, new List<Type>(new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) }));
            in2 = new MultiDataInput(this, new List<Type>(new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) }));
            outp = new AnyTypeOutput(this, typeof(object));

            inputs.Add(in1);
            inputs.Add(in2);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(AddNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Add the 2 inputs together";
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
            return new string[] { "Add", "plus", "+", "function", "math" };
        }

        protected override void DoReset()
        {
            
        }

        protected virtual float FloatOperation(float f1, float f2)
        {
            return f1 + f2;
        }

        protected virtual Vector2 Vec2Operation(Vector2 vec1, Vector2 vec2)
        {
            return vec1 + vec2;
        }

        protected virtual Vector3 Vec3Operation(Vector3 vec1, Vector3 vec2)
        {
            return vec1 + vec2;
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if(in1.GetDataType() == typeof(Vector2))
            {
                //vec2
                Vector2 vec1 = (Vector2)in1.GetData();
                Vector2 vec2 = (Vector2)in2.GetData();
                outp.SetData(Vec2Operation(vec1, vec2));
            }
            else if(in1.GetDataType() == typeof(Vector3))
            {
                //vec3
                Vector3 vec1 = (Vector3)in1.GetData();
                Vector3 vec2 = (Vector3)in2.GetData();
                outp.SetData(Vec3Operation(vec1, vec2));
            }
            else if(in1.GetDataType() == typeof(float))
            {
                //float
                float f1 = (float)in1.GetData();
                float f2 = (float)in2.GetData();
                outp.SetData(FloatOperation(f1, f2));
            }
            else
            {
                Debug.LogError("type fucked in the butt, this shouldn't happen (AddNode)");
            }
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
            if(in1.GetDataType() != null || in2.GetDataType() != null)
            {
                //if there is something plugged in somewhere
                //or both are plugged in
                bool one = true;
                bool two = false;
                Type t = in1.GetDataType();
                if (t == null)
                {
                    one = false;
                    two = true;
                    t = in2.GetDataType();
                }
                if(in2.GetDataType() != null)
                {
                    two = true;
                }
                //t is the right type
                in1.SetAllowedDataTypes(new List<Type>(new Type[] { t }));
                in2.SetAllowedDataTypes(new List<Type>(new Type[] { t }));
                if (one)
                    in1.SetIncommingDataType(t);
                if (two)
                    in2.SetIncommingDataType(t);
                outp.SetDataType(t);
                if (outputs.Count == 0)
                    outputs.Add(outp);
                //both can only accept the right type
            }
            if (in1.GetDataType() == null && in2.GetDataType() == null)
            {
                //otherwise if both are unplugged, open back the inputs to any type
                outputs.Clear();
                in1.SetAllowedDataTypes(new List<Type>(new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) }));
                in2.SetAllowedDataTypes(new List<Type>(new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) }));
            }
            
        }
    }
}

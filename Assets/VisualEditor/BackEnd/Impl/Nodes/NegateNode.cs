using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class NegateNode : Node
    {
        MultiDataInput inp;
        AnyTypeOutput outp;

        public override void BeginSetup()
        {
            inp = new MultiDataInput(this, new List<Type>(new Type[] { typeof(Vector2), typeof(Vector3), typeof(float) }));
            outp = new AnyTypeOutput(this, typeof(object));
            inputs.Add(inp);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(NegateNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Multiplies the input by -1 (minus one)";
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
            return new string[] { "Negate", "*-1" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if (inp.GetDataType() == typeof(Vector2))
            {
                //vec2
                Vector2 vec1 = (Vector2)inp.GetData();
                outp.SetData(Vec2Operation(vec1));
            }
            else if (inp.GetDataType() == typeof(Vector3))
            {
                //vec3
                Vector3 vec1 = (Vector3)inp.GetData();
                outp.SetData(Vec3Operation(vec1));
            }
            else if (inp.GetDataType() == typeof(float))
            {
                //float
                float f1 = (float)inp.GetData();
                outp.SetData(FloatOperation(f1));
            }
            else
            {
                Debug.LogError("type fucked in the butt, this shouldn't happen (NegateNode)");
            }
            processAllOutputs = true;
        }

        private object FloatOperation(float f1)
        {
            return f1 * -1f;
        }

        private object Vec3Operation(Vector3 vec1)
        {
            return vec1 * -1f;
        }

        private object Vec2Operation(Vector2 vec1)
        {
            return vec1 * -1f;
        }

        internal override void PartialSetup()
        {
            if(inp.GetDataType() != null)
            {
                outp.SetDataType(inp.GetDataType());
                if(outputs.Count == 0)
                    outputs.Add(outp);
            }
            if (inp.GetDataType() == null)
                outputs.Clear();
        }
    }
}

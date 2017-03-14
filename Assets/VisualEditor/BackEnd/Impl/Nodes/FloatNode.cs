using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class FloatNode : Node
    {
        public float value = 0f;
        public string strValue = "0";

        public void SetFloat(float f, string s)
        {
            value = f;
            strValue = s;
        }

        public override void BeginSetup()
        {
            outputs.Add(new FloatOutput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(FloatNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(float) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] {"Decimal", "float", "constant" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            outputs[0].SetData(value);
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
        }

        public override string GetDocumentation()
        {
            return "Constantly output the decimal value you typed inside the cut little box.";
        }
    }
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using VisualEditor.Visuals.Impl;
using VisualEditor.Visuals;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class BoolNode : Node
    {
        public bool output = true;

        public void SetOutputValue(bool value)
        {
            output = value;
        }

        public override void BeginSetup()
        {
            outputs.Add(new BoolOutput(this));
        }

        internal override void PartialSetup()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            outputs[0].SetData(output);
            processAllOutputs = true;
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(bool) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Boolean", "true/false", "constant" };
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(BoolNodeVisual);
        }

        protected override void DoReset()
        {
        }

        public override string GetDocumentation()
        {
            return "Constantly output true/false";
        }
    }
}


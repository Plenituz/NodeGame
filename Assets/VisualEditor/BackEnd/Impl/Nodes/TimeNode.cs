using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class TimeNode : Node
    {
        public override void BeginSetup()
        {
            outputs.Add(new FloatOutput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(TimeNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Constantly output the time\nsince the begin of the game";
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
            return new string[] { "Time" };
        }

        protected override void DoReset()
        {
            
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            outputs[0].SetData(Time.time);
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
            
        }
    }
}

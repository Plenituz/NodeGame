using System;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class DebugNode : Node
    {
        public override void BeginSetup()
        {
            inputs.Add(new AnyTypeInput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(DebugNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Print whatever you feed in in the console, be carefull this can drastically slow the game if you spam the console. This node is ignored when you use the spell outside the spell editor.";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(object) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Debug", "log" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            Debug.Log(inputs[0].GetData());
            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {
            
        }
    }
}

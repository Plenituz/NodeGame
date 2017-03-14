using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class GroupOutputNode : Node
    {
        public AnyTypeInput inp;
        public string outName = "output";
        public GroupNode host;
        [NonSerialized]
        public bool hasData = false;

        [NonSerialized]
        public object data;

        public override void BeginSetup()
        {
            inp = new AnyTypeInput(this);
            inputs.Add(inp);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(GroupOutputNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "AKEU KOUKOU BOB. TODO : change that";
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
            return new string[] { "Group output" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            data = inp.GetData();
            hasData = true;
            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {
        }
    }
}

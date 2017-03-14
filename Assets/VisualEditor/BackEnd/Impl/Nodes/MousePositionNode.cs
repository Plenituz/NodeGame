using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class MouseDataNode : Node
    {
        public override void BeginSetup()
        {
            outputs.AddRange(new Output[] { new Vector2Output(this), new BoolOutput(this), new BoolOutput(this) });
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(MouseDataNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Constantly output data related to the mouse cursor : Position and left and right click";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(float), typeof(Vector2) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] {"Mouse data", "click"};
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            Vector2 vect = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            outputs[0].SetData(vect);
            outputs[1].SetData(UnityEngine.Input.GetMouseButton(0));
            outputs[2].SetData(UnityEngine.Input.GetMouseButton(1));
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
        }
    }
}

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
    public class EntityLookNode : Node
    {
        GameObjectInput inObj;
        Vector2Output outVect;

        public override void BeginSetup()
        {
            inObj = new GameObjectInput(this);
            outVect = new Vector2Output(this);

            inputs.Add(inObj);
            outputs.Add(outVect);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(EntityLookNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Get the vector representing where the GameObject is looking";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(GameObject) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(Vector2) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Look", "GameObject Look", "Entity Look", "forward", "vector", "torwards", "function" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            GameObject target = (GameObject)inObj.GetData();
            Vector2 vect = Utils.Forward2D(target.transform);

            outVect.SetData(vect);
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
        }
    }
}

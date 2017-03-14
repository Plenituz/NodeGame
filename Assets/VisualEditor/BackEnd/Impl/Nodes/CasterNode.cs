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
    public class CasterNode : Node
    {
        [NonSerialized]
        private GameObject caster;

        public override void BeginSetup()
        {
            outputs.Add(new GameObjectOutput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(CasterNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(GameObject) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Caster", "this", "me", "entity", "Game Object", "constant" };
        }

        protected override void DoReset()
        {
            
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if(caster == null)
            {
                caster = GameObject.FindGameObjectWithTag("Player");
            }
            if(caster != null)
            {
                outputs[0].SetData(caster);
                processAllOutputs = true;
            }
            else
            {
                processAllOutputs = false;
            }
        }

        internal override void PartialSetup()
        {
            
        }

        public override string GetDocumentation()
        {
            return "Constantly output a GameObject representing your character";
        }
    }
}

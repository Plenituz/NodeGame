using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class DealDamageNode : Node
    {
        public override void BeginSetup()
        {
            inputs.Add(new GameObjectInput(this));
            inputs.Add(new FloatInput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(DealDamageNodeVisual);  
        }

        public override string GetDocumentation()
        {
            return "Deal damage to the character, if possible. The more power you put, the more it will cost";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(GameObject), typeof(float) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Deal damage", "hit" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            GameObject target = (GameObject) inputs[0].GetData();
            float power = (float)inputs[1].GetData();

            Damageable damageable = target.GetComponent<Damageable>();
            if(damageable != null)
            {
                damageable.DealDamage((int)power);
            }
            else
            {
                Debug.Log("this target is not damageable (" + target + ")");
            }
            //TODO take in account player's level

            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {
        }
    }
}

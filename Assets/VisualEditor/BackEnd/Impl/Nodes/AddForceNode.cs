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
    public class AddForceNode : Node
    {
        GameObjectInput inObj;
        Vector2Input inVect;

        public ForceMode2D forceMode = ForceMode2D.Impulse;
        public string forceModestr;

        public void SetForceMode(ForceMode2D mode)
        {
            forceMode = mode;
            forceModestr = mode.ToString();
        }

        public override void BeginSetup()
        {
            inObj = new GameObjectInput(this);
            inVect = new Vector2Input(this);

            inputs.Add(inObj);
            inputs.Add(inVect);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(AddForceNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(GameObject), typeof(Vector2) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] {"Add Force", "push", "move", "action" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            GameObject target = (GameObject)inObj.GetData();
            Vector2 force = (Vector2)inVect.GetData();
            Rigidbody2D rigid = null;
            try {
                rigid = target.GetComponent<Rigidbody2D>();
            }
            catch (NullReferenceException) {}

            if(rigid == null)
            {
                Debug.LogError("target doesn't have a rigid body");
            }
            else
            {
                rigid.AddForce(force, forceMode);
            }
            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {
        }

        public override string GetDocumentation()
        {
            return
                "Add a force to the gameObject, if it's movable.\nThe can be applied either as a pulse or as a push :\n\tPulse : kind of like a punch in da face\n\tPush : more like a gentle push"+
                "\n\nNote that if you feed a constant flow of data the forces are going to accumulate";
        }
    }
}

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
    public class EntityDataNode : Node
    {
        //position, rotation, velocity
        Vector2Output pos, vel;
        Vector3Output rot;
        GameObjectInput inp;

        public override void BeginSetup()
        {
            pos = new Vector2Output(this);
            rot = new Vector3Output(this);
            vel = new Vector2Output(this);
            outputs.AddRange(new Output[] { pos, rot, vel });

            inp = new GameObjectInput(this);
            inputs.Add(inp);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(EntityDataNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(Vector2), typeof(Vector3) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Object data", "position", "rotation", "velocity",  "entity data"};
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            GameObject obj = (GameObject)inp.GetData();
            Vector2 vectPos = obj.transform.position;
            Vector3 vectRot = obj.transform.rotation.eulerAngles;

            Rigidbody2D rigid = null;
            try
            {
                rigid = obj.GetComponent<Rigidbody2D>();
            }
            catch (NullReferenceException) { }
            Vector2 vectVel = rigid == null ? Vector2.zero : rigid.velocity;

            pos.SetData(vectPos);
            rot.SetData(vectRot);
            vel.SetData(vectVel);

            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
        }

        public override string GetDocumentation()
        {
            return "Allow you to get data out of a GameObject : the position, rotation and velocity (speed).";
        }
    }
}

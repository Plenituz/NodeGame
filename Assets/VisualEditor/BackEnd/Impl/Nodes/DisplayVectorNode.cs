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
    class DisplayVectorNode : Node
    {
        Vector2Input vector, at;
        [NonSerialized]
        LineRenderer lr;
        [NonSerialized]
        bool hasProcessed = false;
        [NonSerialized]
        bool hasReset = true;

        public override void BeginSetup()
        {
            vector = new Vector2Input(this);
            at = new Vector2Input(this);
            inputs.Add(vector);
            inputs.Add(at);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(DisplayVectorNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(Vector2) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] {"Display vector", "debug" };
        }

        protected override void DoReset()
        {
            if(lr != null)
                lr.SetVertexCount(0);
        }

        public override void Delete()
        {
            if(lr != null)
                GameObject.Destroy(lr.gameObject);
        }

        protected override void OnUpdate()
        {
            if (lr == null)
            {
                GameObject lrHolder = new GameObject("Draw Vector");
                lr = lrHolder.AddComponent<LineRenderer>();
                lr.SetWidth(0.1f, 0.1f);
            }

            if (!hasProcessed)
            {
                if (!hasReset)
                {
                    lr.SetVertexCount(0);
                    hasReset = true;
                }
            }else
            {
                hasProcessed = false;
            }
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if (lr == null)
            {
                GameObject lrHolder = new GameObject("Draw Vector");
                lr = lrHolder.AddComponent<LineRenderer>();
                lr.SetWidth(0.1f, 0.1f);
            }
            if (lr == null)
            {
                processAllOutputs = false;
                return;
            }
            Vector2 vect = (Vector2)vector.GetData();
            Vector2 pos = (Vector2)at.GetData();
            Debug.DrawLine(pos, pos + vect);
            lr.SetVertexCount(2);
            lr.SetPositions(new Vector3[] { pos, pos + vect });

            //display vector in game
            processAllOutputs = false;
            hasProcessed = true;
            hasReset = false;
        }

        internal override void PartialSetup()
        {
        }

        public override string GetDocumentation()
        {
            return "Draw the Vector fed in the first input at the position fed in the second input, using the color choosed (or fed in th third input)";
        }
    }
}

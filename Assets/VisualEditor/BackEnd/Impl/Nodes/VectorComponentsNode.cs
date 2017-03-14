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
    public class VectorComponentsNode : Node
    {
        MultiDataInput in1;
        FloatOutput x, y, z, magn;

        [NonSerialized]
        private Type oldIn;

        public override void BeginSetup()
        {
            x = new FloatOutput(this);
            y = new FloatOutput(this);
            z = new FloatOutput(this);
            magn = new FloatOutput(this);

            in1 = new MultiDataInput(this, new List<Type>(new Type[] { typeof(Vector2), typeof(Vector3) }));
            inputs.Add(in1);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(VectorComponentsNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(Vector2), typeof(Vector3) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(float) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] {"Vector Components", "magnitude", "modulus", "angle", "x,y,z" };
        }

        protected override void DoReset()
        {
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if(in1.GetDataType() == typeof(Vector2))
            {
                //vector2
                Vector2 vect = (Vector2)in1.GetData();
                x.SetData(vect.x);
                y.SetData(vect.y);
                magn.SetData(vect.magnitude);
            }
            else
            {
                //vector3
                Vector3 vect = (Vector3)in1.GetData();
                x.SetData(vect.x);
                y.SetData(vect.y);
                z.SetData(vect.z);
                magn.SetData(vect.magnitude);
            }
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
            if(in1.GetDataType() == null)
            {
                //nothing plugged in
                outputs.Clear();
                oldIn = null;
            }
            else
            {
                //something is plugged in
                if(in1.GetDataType() != oldIn)
                {
                    //the input type changed
                    oldIn = in1.GetDataType();
                    outputs.Clear();
                    if(in1.GetDataType() == typeof(Vector2))
                    {
                        //vector2
                        outputs.AddRange(new Output[] { x, y, magn });
                    }
                    else
                    {
                        //vector3
                        outputs.AddRange(new Output[] { x, y, z, magn });
                    }
                }
            }
        }

        public override string GetDocumentation()
        {
            return "Separate the vector in the first input into it's components";
        }
    }
}

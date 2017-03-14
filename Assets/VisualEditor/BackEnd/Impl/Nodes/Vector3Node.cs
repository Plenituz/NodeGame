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
    public class Vector3Node : Node
    {
        public Position3 data = new Position3(0, 0, 0);
        public string strX = "0", strY = "0", strZ = "0";

        MultiDataInput inX;
        MultiDataInput inY;
        MultiDataInput inZ;
        bool checkManuallyX = true;
        bool checkManuallyY = true;
        bool checkManuallyZ = true;
        [NonSerialized]
        bool hasProcessedThisFrame = false;

        public void SetVectorX(float x, string s)
        {
            data.x = x;
            strX = s;
        }

        public void SetVectorY(float y, string s)
        {
            data.y = y;
            strY = s;
        }

        public void SetVectorZ(float z, string s)
        {
            data.z = z;
            strZ = s;
        }

        public override void BeginSetup()
        {
            outputs.Add(new Vector3Output(this));

            inX = new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) }));
            inY = new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) }));
            inZ = new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) }));
            inputs.AddRange(new Input[] { inX, inY, inZ });
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(Vector3NodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(Vector3) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Vector3", "direction", "matrix", "coordinates", "constant", "point" };
        }

        protected override void DoReset()
        {
        }

        protected override void OnUpdate()
        {
            if (!hasProcessedThisFrame)
            {
                if (checkManuallyX || checkManuallyY || checkManuallyZ)
                {
                    //if any of the inputs are not filled
                    if (!checkManuallyX && inX.hasData)
                    {
                        //there is smtg plugged in X
                        data.x = (float)inX.GetData();
                        inX.hasData = false;
                    }
                    if (!checkManuallyY && inY.hasData)
                    {
                        //there is smtg plugged in Y
                        data.y = (float)inY.GetData();
                        inY.hasData = false;
                    }
                    if(!checkManuallyZ && inZ.hasData)
                    {
                        data.z = (float)inZ.GetData();
                        inZ.hasData = false;
                    }
                }
                outputs[0].SetData(data.ToVector());
                ProcessAllOutputs();
            }
            hasProcessedThisFrame = false;
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            data = new Position3(
                (float)inX.GetData(), 
                (float)inY.GetData(), 
                (float) inZ.GetData());
            outputs[0].SetData(data.ToVector());
            hasProcessedThisFrame = true;
            processAllOutputs = true;
        }

        internal override void PartialSetup()
        {
            if (inX.GetDataType() == null)
                checkManuallyX = true;
            else
                checkManuallyX = false;

            if (inY.GetDataType() == null)
                checkManuallyY = true;
            else
                checkManuallyY = false;

            if (inZ.GetDataType() == null)
                checkManuallyZ = true;
            else
                checkManuallyZ = false;
        }

        public override string GetDocumentation()
        {
            return "Just like you make a cake with flour, you can make a vector with this node.";
        }
    }  
}

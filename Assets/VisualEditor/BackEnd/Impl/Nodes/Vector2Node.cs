using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class Vector2Node : Node
    {
        public Position2 data = new Position2(0f, 0f);
        public string strX = "0";
        public string strY = "0";
        MultiDataInput inX;
        MultiDataInput inY;
        bool checkManuallyX = true;
        bool checkManuallyY = true;
        [NonSerialized]
        bool hasProcessedThisFrame = false;
        
        public override void BeginSetup()
        {
            outputs.Add(new Vector2Output(this));

            inX = new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) }));
            inY = new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) }));
            inputs.Add(inX);
            inputs.Add(inY);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(Vector2NodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(Vector2) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] {"Vector2", "direction", "matrix", "coordinates", "constant", "point" };
        }

        public void SetVectorX(float v, string s)
        {
            data.x = v;
            strX = s;
        }

        public void SetVectorY(float v, string s)
        {
            data.y = v;
            strY = s;
        }

        protected override void OnUpdate()
        {
            if (!hasProcessedThisFrame)
            {
                if (checkManuallyX || checkManuallyY)
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
                }
                outputs[0].SetData(data.ToVector());
                ProcessAllOutputs();
            }
            hasProcessedThisFrame = false;
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            //we know both inputs are filled if this is called
            data = new Position2((float)inX.GetData(), (float)inY.GetData());
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
        }

        protected override void DoReset()
        {
        }

        public override string GetDocumentation()
        {
            return "Constantly output the Vector created by your tiny hands.";
        }
    }
}

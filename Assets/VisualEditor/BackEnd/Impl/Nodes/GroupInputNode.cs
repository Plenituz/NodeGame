using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class GroupInputNode : Node
    {
        AnyTypeOutput outp;
        public Type dataType;
        public string inName = "input";
        public GroupNode host;

        [NonSerialized]
        object data;

        public void SetData(object data)
        {
            if (data.GetType() == dataType)
                this.data = data;
            else
                Debug.LogError("Data types dont match");
        }

        public void SetDataType(Type t)
        {
            dataType = t;
            if(outp == null)
            {
                outp = new AnyTypeOutput(this, t);
                outputs.Add(outp);
            }
            else
            {
                outp.SetDataType(t);
            }
        }

        public override void BeginSetup()
        {
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(GroupInputNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "This is only useful in a group. This will create an input on the group node and you can get data going to this input from this node.";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(object) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Group input" };
        }

        protected override void DoReset()
        {

        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if(data != null && outp != null)
            {
                outp.SetData(data);
                data = null;
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
    }
}

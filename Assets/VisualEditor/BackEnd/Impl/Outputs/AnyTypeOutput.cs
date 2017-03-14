using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class AnyTypeOutput : Output
    {
        [NonSerialized]
        private object data;
        private Type dataType;

        public AnyTypeOutput(Node node, Type type) : base(node)
        {
            SetDataType(type);
        }

        public void SetDataType(Type type)
        {
            if(type != dataType)
            {
                DisconnectFromAll();
            }
            dataType = type;
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return dataType;
        }

        internal override void SetData(object data)
        {
            this.data = data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class Vector2Input : Input
    {
        [NonSerialized]
        Vector2 data;

        public Vector2Input(Node node) : base(node)
        {

        }

        internal override List<Type> GetAllowedDataTypes()
        {
            return new List<Type>(new Type[] { typeof(Vector2) });
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(Vector2);
        }

        protected override void DoSetData(object data, Type dataType, out bool hasdata)
        {
            this.data = (Vector2)data;
            hasdata = true;
        }

        internal override void SetIncommingDataType(Type type)
        {
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class MultiDataInput : Input
    {
        List<Type> allowedDataTypes = new List<Type>();
        [NonSerialized]
        object data;
        Type actualType;

        public MultiDataInput(Node node, List<Type> allowedDataTypes) : base(node)
        {
            this.allowedDataTypes = allowedDataTypes;
        }

        internal void SetAllowedDataTypes(List<Type> allowed)
        {
            actualType = null;//when the allowed data type changes, reset the actual type
            this.allowedDataTypes = allowed;
        }

        internal override List<Type> GetAllowedDataTypes()
        {
            return actualType == null ? allowedDataTypes : new List<Type>(new Type[] { actualType });
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return actualType;
        }

        protected override void DoSetData(object data, Type dataType, out bool hasdata)
        {
            this.data = data;
            actualType = dataType;
            hasdata = true;
        }

        internal override void SetIncommingDataType(Type type)
        {
            actualType = type;
        }
    }
}

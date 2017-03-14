using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class ObjectInput : Input
    {
        [NonSerialized]
        private object data;

        public ObjectInput(Node node) : base(node)
        {

        }

        internal override List<Type> GetAllowedDataTypes()
        {
            return new List<Type>(new Type[] { typeof(object) });
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(object);
        }

        protected override void DoSetData(object data, Type dataType, out bool hasdata)
        {
            this.data = data;
            hasdata = true;
        }

        internal override void SetIncommingDataType(Type type)
        {
            
        }
    }
}

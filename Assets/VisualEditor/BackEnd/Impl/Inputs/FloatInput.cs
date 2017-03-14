using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class FloatInput : Input
    {
        [NonSerialized]
        float data;

        public FloatInput(Node node) : base(node)
        {

        }

        internal override List<Type> GetAllowedDataTypes()
        {
            return new List<Type>(new Type[] { typeof(float) });
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(float);
        }

        protected override void DoSetData(object data, Type dataType, out bool hasdata)
        {
            this.data = (float)data;
            hasdata = true;
        }

        internal override void SetIncommingDataType(Type type)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class BoolInput : Input
    {
        [NonSerialized]
        bool data;

        public BoolInput(Node node) : base(node)
        {

        }

        internal override List<Type> GetAllowedDataTypes()
        {
            return new List<Type>(new Type[] { typeof(bool) });
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(bool);
        }

        protected override void DoSetData(object data, Type dataType, out bool hasdata)
        {
            this.data = (bool)data;
            hasdata = true;
        }

        internal override void SetIncommingDataType(Type type)
        {
        }
    }
}

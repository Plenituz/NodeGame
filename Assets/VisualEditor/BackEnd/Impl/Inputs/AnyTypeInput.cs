using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class AnyTypeInput : Input
    {
        [NonSerialized] private object data;
        private Type dataType;

        public AnyTypeInput(Node node) : base(node)
        {

        }

        internal override List<Type> GetAllowedDataTypes()
        {
            return dataType == null ? new List<Type>(new Type[] { typeof(object) }) : new List<Type>(new Type[] { dataType });
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return dataType;
        }

        protected override void DoSetData(object data, Type dataType, out bool hasdata)
        {
            this.data = data;
            this.dataType = dataType;
            hasdata = true;
        }

        internal override void SetIncommingDataType(Type type)
        {
            dataType = type;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class FloatOutput : Output
    {
        [NonSerialized]
        public float data;

        public FloatOutput(Node node) : base(node)
        {

        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(float);
        }

        internal override void SetData(object data)
        {
            this.data = (float)data;
        }
    }
}

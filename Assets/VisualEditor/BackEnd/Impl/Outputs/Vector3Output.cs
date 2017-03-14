using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class Vector3Output : Output
    {
        [NonSerialized]
        Vector3 data;

        public Vector3Output(Node node) : base(node)
        {

        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(Vector3);
        }

        internal override void SetData(object data)
        {
            this.data = (Vector3)data;
        }
    }
}

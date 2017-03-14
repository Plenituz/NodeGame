using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class Vector2Output : Output
    {
        [NonSerialized]
        public Vector2 data;

        public Vector2Output(Node node) : base(node)
        {

        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(Vector2);
        }

        internal override void SetData(object data)
        {
            this.data = (Vector2)data;
        }
    }
}

using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class BoolOutput : Output
    {
        [NonSerialized]
        bool data;

        public BoolOutput(Node node) : base(node)
        {
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(bool);
        }

        internal override void SetData(object data)
        {
            try
            {
                this.data = (bool)data;
            }
            catch (InvalidCastException)
            {
                Debug.LogError("could not cast from " + data.GetType() + " to bool in " + host.GetType());
            }
        }
    }
}

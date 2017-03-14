using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class GameObjectOutput : Output
    {
        [NonSerialized]
        GameObject data;

        public GameObjectOutput(Node node) : base(node)
        {

        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(GameObject);
        }

        internal override void SetData(object data)
        {
            this.data = (GameObject)data;
        }
    }
}

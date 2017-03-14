using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public class GameObjectInput : Input
    {
        [NonSerialized]
        private GameObject data;

        public GameObjectInput(Node node) : base(node)
        {

        }

        internal override List<Type> GetAllowedDataTypes()
        {
            return new List<Type>(new Type[] { typeof(GameObject)});
        }

        internal override object GetData()
        {
            return data;
        }

        internal override Type GetDataType()
        {
            return typeof(GameObject);
        }

        protected override void DoSetData(object data, Type dataType, out bool hasdata)
        {
            this.data = (GameObject)data;
            hasdata = true;
        }

        internal override void SetIncommingDataType(Type type)
        {
            
        }
    }
}

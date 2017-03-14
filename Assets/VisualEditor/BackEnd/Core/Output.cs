using System.Collections.Generic;
using System;
using UnityEngine;
using VisualEditor.BackEnd;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public abstract class Output
    {
        public Output(Node node)
        {
            host = node;
        }

        [NonSerialized]
        public Action<Input> onConnect;
        [NonSerialized]
        public Action<Input> onDisconnect;

        internal Node host;
        internal List<Input> destinations = new List<Input>();//list of inputs thi output leads to, this is set by the UI part (by calling ConnectTo)

        internal abstract Type GetDataType();

        internal abstract void SetData(object data);//store the data to be passed to another input
        //called by the hosting node once the data from his inputs have been processed (in Process())

        internal abstract object GetData();//give back the data you stored to pass to the inputs of other node (stored inside destinations)
        //called by the hosting node at the end of his process

        public virtual bool ConnectTo(Input input)//this is for the UI part
        {
            if (GetDataType() == null)//if i don't know what i'am returning dont create link
                return false;
            if (!ContainsType(input.GetAllowedDataTypes(), GetDataType()))//if i am not the right data type don't allow connection
                return false;
            destinations.Add(input);
            input.SetIncommingDataType(GetDataType());
            input.host.PartialSetup();
            input.outputConnectedTo = this;
            if (onConnect != null)
                onConnect(input);
            return true;
        }

        /// <summary>
        /// connect to input without sending a partial setup
        /// </summary>
        /// <param name="input">input to connect to</param>
        /// <returns></returns>
        public virtual bool ConnectToDiscreet(Input input)
        {
           // if (GetDataType() == null)//if i don't know what i'am returning dont create link
          //      return false;
           // if (!ContainsType(input.GetAllowedDataTypes(), GetDataType()))//if i am not the right data type don't allow connection
           //     return false;
            destinations.Add(input);
            input.SetIncommingDataType(GetDataType());
            //input.host.PartialSetup();
            input.outputConnectedTo = this;
          //  if (onConnect != null)
            //    onConnect(input);
            return true;
        }

        public virtual void DisconnectFrom(Input input)
        {
            input.SetIncommingDataType(null);
            if (!destinations.Remove(input))
            {
                Debug.LogError("Trying to disconnect an input that was never connected in " + GetType());
            }
            input.host.PartialSetup();
            if (onDisconnect != null)
                onDisconnect(input);
        }

        public void DisconnectFromAll()
        {
            while(destinations.Count != 0)
            {
                DisconnectFrom(destinations[0]);
            }
        }

        public static bool ContainsType(List<Type> list, Type t)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if(t.IsSubclassOf(list[i]) || t == list[i])
                    return true;
            }
            return false;
        }
    }
}

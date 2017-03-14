using System.Collections.Generic;
using System;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public abstract class Input
    {
        public Input(Node node)
        {
            host = node;
        }

        internal Node host;
        internal Output outputConnectedTo;
        internal bool hasData;//if all the inputs in a node are ready, then the node will process the info

        internal abstract List<Type> GetAllowedDataTypes();//returns the possible data types this input can take, this is used to check if the input can receive a link
        internal abstract Type GetDataType();//this returns the actual type the data is going to be
        //this can only be called AFTER SetData has been called, therefore to be secure it can only be called when the input is ready 

        internal void SetData(object data, Type dataType)
        {
            DoSetData(data, dataType, out hasData);
        }
        protected abstract void DoSetData(object data, Type dataType, out bool hasData);//save the data as you want, and don't forget to set hasData to true when the input has enough data for the node to process
        //called by a node you are not into


        internal abstract object GetData();//returns the data saved by SetData
        //called by the host node to process the data

        internal abstract void SetIncommingDataType(Type type);//this is for when the UI connect but the spell is not running so that you can call GetDataType in PartialSetup
        //CAREFUL : type here can be NULL ! (and will be)

    }
}

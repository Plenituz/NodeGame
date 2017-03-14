using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using VisualEditor.Visuals;
using System.Reflection;

namespace VisualEditor.BackEnd
{
    [Serializable]//DONT FORGET TO PUT SERIALIZABLE WHEN EXTENDING THIS CLASS
    public abstract class Node
    {
        public bool enabled = true;
        [NonSerialized]
        public Spell hostSpell;
        public Action onProcessOutput;
        public Action onProcessInputsCalled;
        public string comment;

        internal List<Input> inputs = new List<Input>();
        internal List<Output> outputs = new List<Output>();
        //internal bool done = false;//used by monobehaviour that is updating the node to check if it needs to be updated or not

        protected abstract void ProcessInputs(out bool processAllOutputs);//take data from the input, process it and update data on the outputs (ProcessAllOutputs)
                                          //this should set done to true when the node is done and don't need updates anymore
                                          //and should also call ProcessAllOutputs to allow the following nodes to process there shit, or set processAllOutputs to true
                                          //or you can call ProcessOutput by hand

        public abstract void BeginSetup();//called once when the node is created, used to setup thing that can't change
        public virtual void Delete()
        {

        }
        internal abstract void PartialSetup();//called every time an input in the node get updated
        public abstract Type[] GetPossibleInputTypes();//used by the TabMenu to determine what to displayed base on input or output type
        public abstract Type[] GetPossibleOutputTypes();
        public abstract string[] GetSearchableNames();
        public abstract Type GetAssociatedVisualClass();
        public abstract string GetDocumentation();

        public virtual void SetComment(string s)
        {
            comment = s;
        }
        public virtual string GetComment()
        {
            return comment;
        }

        public virtual InputManagerSerializer GetExample() {
            return InputManagerSerializer.LoadFromDisk(Application.dataPath + "/Spells/Examples/" + TabMenu.CleanClassName(GetType().ToString()) + ".SpellDic");
        }

        protected abstract void DoReset();//this is called at the end of every run and should reset the node values, note that it is not called hen the node is deleted
        //for that use Delete()

        public void Reset()
        {
            //done = false;
            for(int i = 0; i < inputs.Count; i++)
            {
                inputs[i].hasData = false;
            }
            DoReset();
        }

        protected virtual void OnUpdate()
        {
            
        }

        public void StopSpell()
        {
            hostSpell.Stop();
        }

        public void Update()
        {
            if (!enabled)
                return;
            if (inputs.Count == 0)
            {
                //if there is no input, process it everytime
                bool processAllOutputs;
                ProcessInputs(out processAllOutputs);
                if (onProcessInputsCalled != null)
                    onProcessInputsCalled();
                if (processAllOutputs)
                    ProcessAllOutputs();
            }
            else
            {
                //there are some inputs
                //process the inputs and clear the hasData tags
                bool allData = true;
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (!inputs[i].hasData)
                    {
                        allData = false;
                        break;
                    }
                }
                if (allData)
                {
                    bool processAllOutputs;
                    ProcessInputs(out processAllOutputs);
                    if (onProcessInputsCalled != null)
                        onProcessInputsCalled();
                    if (processAllOutputs)
                        ProcessAllOutputs();
                    for(int i = 0; i < inputs.Count; i++)
                    {
                        inputs[i].hasData = false;
                        //might want to clear the data as well as just setting this to false
                        //if there is memory issues
                    }
                }
            }
            OnUpdate();
        }


        /**
         * from is the output of and other node and to is the input on this node to update with from's data
         */
        protected bool ProcessOutput(Output from, Input to)
        {
            if (!enabled)
                return false;
            if (Output.ContainsType(to.GetAllowedDataTypes(), from.GetDataType()))//if the input and output are compatible, proceed
            {
                object data = from.GetData();
                to.SetData(data, from.GetDataType());
                return true;
            }
            else
            {
                Debug.LogError("output and input types are not compatible : " + from.host.GetType() + " to : " + to.host.GetType());
                return false;
            }
        }

        protected void ProcessSingleOutput(Output o)
        {
            if (onProcessOutput != null)
                onProcessOutput();
            for (int i = 0; i < o.destinations.Count; i++)
            {
                ProcessOutput(o, o.destinations[i]);
            }
        }

        protected void ProcessAllOutputs()
        {
            if (onProcessOutput != null)
                onProcessOutput();
            for(int o = 0; o < outputs.Count; o++)
            {
                for(int i = 0; i < outputs[o].destinations.Count; i++)
                {
                    ProcessOutput(outputs[o], outputs[o].destinations[i]);
                }
            }
        }
    }
}

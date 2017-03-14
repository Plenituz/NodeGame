using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class IsProcessingListenerNode : ListenerBaseNode
    {
        [NonSerialized]
        private bool hasProcessed = false;

        public override void BeginSetup()
        {
            outputs.Add(new BoolOutput(this));
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(IsProcessingListenerNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "outputs true when the target node is processing";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(bool) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Process listener", "has data" };
        }

        void OnEvent()
        {
            hasProcessed = true;
            outputs[0].SetData(true);
            ProcessSingleOutput(outputs[0]);
        }

        public override void ListenTo(Node node)
        {
            Debug.Log("listening to " + node);
            listenTo.onProcessOutput += OnEvent;
            listenTo.onProcessInputsCalled += OnEvent;
        }

        public override void StopListening()
        {
            Debug.Log("Stop listen");
            if(listenTo != null)
            {
                listenTo.onProcessOutput -= OnEvent;
                listenTo.onProcessInputsCalled -= OnEvent;
            }
        }

        protected override void DoReset()
        {

        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            if (hasProcessed)
            {
                hasProcessed = false;
                processAllOutputs = false;
            }
            else
            {
                outputs[0].SetData(false);
                processAllOutputs = true;
            }
        }

        internal override void PartialSetup()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class GroupNode : Node
    {
        //set on create
        public InputManagerSerializer serializer = new InputManagerSerializer();//this is set on node creation, the group is always created with nodes inside
                
        //set by self
        [NonSerialized]
        public InputManager group;//create that at some point with groupMode = true et levelU^pifGRoup = host et name et tout
        public string groupName = "Group";//this has to be updated when the group name get updated

        [NonSerialized]
        public List<GroupInputNodeVisual> groupIns = new List<GroupInputNodeVisual>();
        [NonSerialized]
        public List<GroupOutputNodeVisual> groupOuts = new List<GroupOutputNodeVisual>();
        [NonSerialized]
        bool hasProcessed = false;

        public override void BeginSetup()
        {
            GameObject go = new GameObject("InputManager(group) - groupName");
            group = go.AddComponent<InputManager>();
            group.groupMode = true;
            group.hostIfGroup = this;
            serializer.PrepareForSerialization();
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(GroupNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Group of node";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { };
        }

        protected override void DoReset()
        {
            //reset all nodes in serialiser
            serializer.PrepareForSerialization();
            for(int i = 0; i < serializer.nodes.Length; i++)
            {
                serializer.nodes[i].Reset();
            }
        }

        protected override void OnUpdate()
        {
            if (!hasProcessed)
            {
                for (int i = 0; i < groupIns.Count; i++)
                {//set data from inputs to inputs nodes
                    if (inputs[i].hasData)
                    {
                        ((GroupInputNode)groupIns[i].node).SetData(inputs[i].GetData());
                        inputs[i].hasData = false;
                    }
                }
            }
            //updates nodes
            for (int i = 0; i < serializer.nodes.Length; i++)
            {
                //if this if a groupe input, give it data 
                serializer.nodes[i].Update();
                //if this is a groupe output, get the data
            }
            //set data from output node to outputs
            for (int i = 0; i < groupOuts.Count; i++)
            {
                GroupOutputNode node = (GroupOutputNode)groupOuts[i].node;
                if (node.hasData)
                {
                    outputs[i].SetData(node.data);
                    node.hasData = false;
                    ProcessSingleOutput(outputs[i]);
                }
            }
            
            hasProcessed = false;
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            for (int i = 0; i < groupIns.Count; i++)
            {
                if (inputs[i].hasData)
                {
                    ((GroupInputNode)groupIns[i].node).SetData(inputs[i].GetData());
                    inputs[i].hasData = false;
                }
            }
            hasProcessed = true;
            processAllOutputs = false;
        }

        public void AddNode(NodeVisual n)
        {
            GameObject tmp = new GameObject("tmp");
            WaitAndDo t = tmp.AddComponent<WaitAndDo>();
            t.arg = n;
            t.act = (object arg) =>
            {
                NodeVisual node = (NodeVisual)arg;
                serializer.Add(node.node, new Position2(node.transform.position));
                serializer.PrepareForSerialization();
                if (node.GetType() == typeof(GroupInputNodeVisual))
                {
                    //group input
                    GroupInputNodeVisual nn = (GroupInputNodeVisual)node;
                    ((GroupInputNode)nn.node).host = this;
                    AddSortedIn(nn);
                    UpdateInputs();
                }
                else if (node.GetType() == typeof(GroupOutputNodeVisual))
                {
                    //group output
                    GroupOutputNodeVisual nn = (GroupOutputNodeVisual)node;
                    ((GroupOutputNode)nn.node).host = this;
                    AddSortedOut(nn);
                    UpdateOutputs();
                }
            };
            //if it's an input/output add it to the list
        }

        public void PopulateGroupInOutFromGroup()
        {
            for(int i = 0; i < serializer.nodes.Length; i++)
            {
                serializer.Add(serializer.nodes[i], serializer.positions[i]);
            }
            groupIns = new List<GroupInputNodeVisual>();
            groupOuts = new List<GroupOutputNodeVisual>();
            for(int i = 0; i < group.allNodes.Count; i++)
            {
                NodeVisual poten;
                if((poten = group.allNodes[i].GetComponent<GroupInputNodeVisual>()) != null)
                {
                    groupIns.Add((GroupInputNodeVisual)poten);
                }
                if((poten = group.allNodes[i].GetComponent<GroupOutputNodeVisual>()) != null)
                {
                    groupOuts.Add((GroupOutputNodeVisual)poten);
                }
            }
            UpdateInputs();
            UpdateOutputs();
        }

        public void UpdateInputs()
        {
            SortIns();
            for(int i = 0; i < groupIns.Count; i++)
            {
                if(inputs.Count > i)
                {
                    //update current input
                    MultiDataInput inp = (MultiDataInput)inputs[i];
                    GroupInputNode node = (GroupInputNode)groupIns[i].node;
                    if (!inp.GetAllowedDataTypes().Contains(node.dataType))
                    {
                        inp.SetAllowedDataTypes(new List<Type>(new Type[] { node.dataType }));
                    }
                }
                else
                {
                    //create new input
                    GroupInputNode node = (GroupInputNode)groupIns[i].node;
                    MultiDataInput inp = new MultiDataInput(this, new List<Type>(new Type[] { node.dataType }));
                    inputs.Add(inp);
                }
            }
            //supprime les autres inputs

            while (inputs.Count > groupIns.Count)
            {
                inputs.RemoveAt(groupIns.Count);
            }
            
        }

        private void SortIns()
        {
            for (int i = 0; i < groupIns.Count; i++)
            {
                for(int k = i; k < groupIns.Count; k++)
                {
                    if(groupIns[k].transform.position.y > groupIns[i].transform.position.y)
                    {
                        //swap
                        var tmp = groupIns[k];
                        groupIns[k] = groupIns[i];
                        groupIns[i] = tmp;
                    }
                }
            }
        }

        public void UpdateOutputs()
        {
            SortOuts();
            for (int i = 0; i < groupOuts.Count; i++)
            {
                if (outputs.Count > i)
                {
                    //update current output
                    AnyTypeOutput outp = (AnyTypeOutput)outputs[i];
                    GroupOutputNode node = (GroupOutputNode)groupOuts[i].node;
                    outp.SetDataType(node.inp.GetDataType());
                }
                else
                {
                    //create new output
                    Debug.Log("creating new output");
                    GroupOutputNode node = (GroupOutputNode)groupOuts[i].node;
                    AnyTypeOutput outp = new AnyTypeOutput(this, node.inp.GetDataType());
                    outputs.Add(outp);
                }
            }
            //supprime les autres output
            while (outputs.Count > groupOuts.Count)
            {
                outputs.RemoveAt(groupOuts.Count);
            }
        }

        private void SortOuts()
        {
            for (int i = 0; i < groupOuts.Count; i++)
            {
                for (int k = i; k < groupOuts.Count; k++)
                {
                    if (groupOuts[k].transform.position.y > groupOuts[i].transform.position.y)
                    {
                        //swap
                        var tmp = groupOuts[k];
                        groupOuts[k] = groupOuts[i];
                        groupOuts[i] = tmp;
                    }
                }
            }
        }

        private void AddSortedOut(GroupOutputNodeVisual n)
        {
            for(int i = 0; i < groupOuts.Count; i++)
            {
                if(n.transform.position.y > groupOuts[i].transform.position.y)
                {
                    groupOuts.Insert(i + 1, n);
                    return;
                }
            }
            groupOuts.Add(n);
        }

        private void AddSortedIn(GroupInputNodeVisual n)
        {
            for (int i = 0; i < groupIns.Count; i++)
            {
                if (n.transform.position.y > groupIns[i].transform.position.y)
                {
                    groupIns.Insert(i + 1, n);
                    return;
                }
            }
            groupIns.Add(n);
        }

        internal void RemoveNode(NodeVisual node)
        {
            for(int i = 0; i < serializer.nodes.Length; i++)
            {
                if(serializer.tmpNodes[i] == node.node)
                {
                    serializer.tmpNodes.RemoveAt(i);
                    serializer.tmpPos.RemoveAt(i);
                    serializer.PrepareForSerialization();
                    if(node.GetType() == typeof(GroupOutputNodeVisual))
                    {
                        groupOuts.Remove((GroupOutputNodeVisual)node);
                    }else if(node.GetType() == typeof(GroupInputNodeVisual))
                    {
                        groupIns.Remove((GroupInputNodeVisual)node);
                    }
                    //if it's an input/output remove it from list
                    return;
                }
            }
            Debug.LogError("Couldn't remove node " + node.node);
        }

        public override void Delete()
        {
            serializer.PrepareForSerialization();
            for(int i = 0; i < serializer.nodes.Length; i++)
            {
                serializer.nodes[i].Delete();
            }
            GameObject.Destroy(group.gameObject);
        }

        internal override void PartialSetup()
        {
        }
    }
}

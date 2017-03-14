using UnityEngine;
using System.Collections.Generic;
using System;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class CompareNode : Node
    {
        public const int GREATER_THAN = 1;
        public const int LESSER_THAN = 2;
        public const int GREATER_OR_EQUAL_TO = 3;
        public const int LESSER_OR_EQUAL_TO = 4;
        public const int NOT_EQUAL = 5;
        public const int EQUAL = 6;
        public const int AND = 7;
        public const int OR = 8;
        public const int XOR = 9;

        Type oldIn1Type = null;
        public int[] availableActions;//This is for the UI to display the list of actions
        public int selectedAction = -1;//this has to be set by the UI 

        public void SetOperation(int op)
        {
            selectedAction = op;
        }

        public override void BeginSetup()
        {
            List<Type> allowedIn = new List<Type>();
            allowedIn.AddRange(new Type[] { typeof(float), typeof(bool), typeof(Vector2), typeof(Vector3) });
            MultiDataInput in1 = new MultiDataInput(this, allowedIn);
            inputs.Add(in1);

            outputs.Add(new BoolOutput(this));
        }

        internal override void PartialSetup()
        {
            if (inputs[0].GetDataType() == null)
            {
                //the link to the first input has been disconnected 
                if(inputs.Count > 1)
                    inputs.RemoveAt(1);
                oldIn1Type = null;
                availableActions = null;
               // selectedAction = -1;
                //remove the second input because there is no longer something attached to the first one
                //and escape this function
                return;
            }
            if (oldIn1Type == null || oldIn1Type != inputs[0].GetDataType())
            {
                oldIn1Type = inputs[0].GetDataType();
    
                if (inputs.Count == 1)
                {
                    inputs.Add(new MultiDataInput(this, new List<Type>()));
                }
                //the first input just got filled OR the first input changed data type

                //change available operations and the second input allowed data type
                if (inputs[0].GetDataType() == typeof(float))
                {
                    List<Type> l = new List<Type>();
                    l.Add(typeof(float));
                    ((MultiDataInput)inputs[1]).SetAllowedDataTypes(l);

                    availableActions = new int[] { GREATER_THAN, LESSER_THAN, GREATER_OR_EQUAL_TO, LESSER_OR_EQUAL_TO, NOT_EQUAL, EQUAL };
                }
                else if (inputs[0].GetDataType() == typeof(bool))
                {
                    List<Type> l = new List<Type>();
                    l.Add(typeof(bool));
                    ((MultiDataInput)inputs[1]).SetAllowedDataTypes(l);

                    availableActions = new int[] { AND, OR, XOR, NOT_EQUAL, EQUAL };
                }
                else if (inputs[0].GetDataType() == typeof(Vector2) || inputs[0].GetDataType() == typeof(Vector3))
                {
                    List<Type> l = new List<Type>();
                    l.AddRange(new Type[] { typeof(float), typeof(Vector2), typeof(Vector3) });
                    ((MultiDataInput)inputs[1]).SetAllowedDataTypes(l);

                    availableActions = new int[] { GREATER_THAN, LESSER_THAN, GREATER_OR_EQUAL_TO, LESSER_OR_EQUAL_TO, NOT_EQUAL, EQUAL };
                }
                else
                {//something went wrong, this should not happen
                    Debug.LogError("input data type unexpected : " + inputs[0].GetDataType());
                }
            }//else the second input just got filled
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            //here we have confirmation that every input is filled and ready 
            bool output;
            if (inputs[0].GetDataType() == typeof(float))
            {
                //the 2 inputs are floats, comapre them
                float v1 = (float)inputs[0].GetData();
                float v2 = (float)inputs[1].GetData();
                output = CompareFloats(v1, v2, selectedAction);
            }
            else if (inputs[0].GetDataType() == typeof(bool))
            {
                //the 2 inputs are bools, compare them
                bool v1 = (bool)inputs[0].GetData();
                bool v2 = (bool)inputs[1].GetData();
                output = CompareBools(v1, v2, selectedAction);
            }
            else if (inputs[0].GetDataType() == typeof(Vector2) || inputs[0].GetDataType() == typeof(Vector3))
            {
                //the first input if a vector, the second input is either a vector or a float
                Vector3 in1 = Vector3.zero;
                if (inputs[0].GetDataType() == typeof(Vector2))
                    in1 = (Vector2)inputs[0].GetData();
                else
                    in1 = (Vector3)inputs[0].GetData();

                float v1 = in1.magnitude;
                float v2;
                if (inputs[1].GetDataType() == typeof(float))
                {
                    v2 = (float)inputs[1].GetData();
                }
                else
                {
                    Vector3 in2 = Vector3.zero;
                    if (inputs[1].GetDataType() == typeof(Vector2))
                        in2 = (Vector2)inputs[1].GetData();
                    else
                        in2 = (Vector3)inputs[1].GetData();

                    v2 = in2.magnitude;
                }
                output = CompareFloats(v1, v2, selectedAction);
            }
            else
            {
                Debug.LogError("Data type not expected" + inputs[0].GetDataType());
                processAllOutputs = false;
                return;
            }
            outputs[0].SetData(output);
            processAllOutputs = true;
        }

        bool CompareFloats(float v1, float v2, int operation)
        {
            switch (operation)
            {
                case GREATER_THAN:
                    return v1 > v2;
                case LESSER_THAN:
                    return v1 < v2;
                case GREATER_OR_EQUAL_TO:
                    return v1 >= v2;
                case LESSER_OR_EQUAL_TO:
                    return v1 <= v2;
                case NOT_EQUAL:
                    return v1 != v2;
                case EQUAL:
                    return v1 == v2;
                default:
                    Debug.LogError("Invalid operation on floats : " + operation);
                    return false;
            }
        }

        bool CompareBools(bool v1, bool v2, int operation)
        {
            switch (operation)
            {
                case AND:
                    return v1 && v2;
                case OR:
                    return v1 || v2;
                case XOR:
                    return v1 ^ v2;
                case EQUAL:
                    return v1 == v2;
                case NOT_EQUAL:
                    return v1 != v2;
                default:
                    Debug.LogError("Invalid operation on bools : " + operation);
                    return false;
            }
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(bool), typeof(float), typeof(Vector2), typeof(Vector3) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(bool) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Compare", "if", "AND", "OR", "XOR", "=", "==", "!=", ">", "<", ">=", "<=", "greater than", "lesser than", "greater or equal", "lesser or equal" };
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(CompareNodeVisual);
        }

        protected override void DoReset()
        {
        }

        public override string GetDocumentation()
        {
            return "Compare 2 values according to the instruction you put in.\nThe possible instructions depends on what is fed in the first input:\n\t" +
                "AND, OR, XOR, =, !=, >, <, >=, <=\nIf you feed in a Vector the comparison if done according to the Vector's magnitude (or lenght, ex : the vector (1, 1) has a magnitude of √2)";
        }
    }

}
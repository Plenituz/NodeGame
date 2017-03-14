using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.BackEnd.Impl;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class VariableNode : Node
    {
        [NonSerialized]
        public static Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        [NonSerialized]
        public Action<Type> onTypeChanged;

        public string varName = "";
        public Type varType = typeof(Vector2);

        private MultiDataInput inp;
        private AnyTypeOutput outp;
        Variable subbedTo;

        public override void BeginSetup()
        {
            inp = new MultiDataInput(this, new List<Type>(new Type[] { typeof(Vector2) }));
            outp = new AnyTypeOutput(this, typeof(Vector2));
            outp.onConnect = OnOutputChange;
            outp.onDisconnect = OnOutputChange;

            inputs.Add(inp);
            outputs.Add(outp);
            SetVariableName("");
            //ya pas d'output tant que c'est pas un nom de variable valid
        }

        void OnOutputChange(Input input)
        {
            PartialSetup();
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(VariableNodeVisual);
        }

        public void SetVariableName(string str)
        {
            varName = str;
            
            if (!variables.ContainsKey(varName))
            {
                //si la var existe pas, on la crée et on sub to it
                if (subbedTo != null)
                    subbedTo.UnSubscribe(this);
                Variable nVar = new Variable(varType);
                nVar.Subscribe(this);
                subbedTo = nVar;
                variables.Add(varName, nVar);
                InitFromVar();
            }
            else if(variables[varName] != subbedTo)
            {
                //si elle existe et que c'est pas la meme que nous, on y sub
                if (subbedTo != null)
                    subbedTo.UnSubscribe(this);
                subbedTo = variables[varName];
                subbedTo.Subscribe(this);
                InitFromVar();
            }
        }

        public void InitFromVar()
        {
            if (variables.ContainsKey(varName))
            {
                Variable vari = variables[varName];
                varType = vari.GetDataType();
                inp.SetAllowedDataTypes(new List<Type>(new Type[] { vari.GetDataType() }));
                outp.SetDataType(vari.GetDataType());
                if (varType != null && onTypeChanged != null)
                    onTypeChanged(varType);
            }
        }

        public void SetVariableType(Type t)
        {
            if (!variables.ContainsKey(varName))
            {
                //si la var existe pas, on la crée et on sub to it
                Variable nVar = new Variable(t);
                nVar.Subscribe(this);
                subbedTo = nVar;
                variables.Add(varName, nVar);
            }
            else
            {
                //si elle existe, on change son type
                variables[varName].SetDataType(t);
            }
        }

        public void OnVariableTypeChanged()
        {
            InitFromVar();
            if (varType != null && onTypeChanged != null)
                onTypeChanged(varType);
        }

        public override string GetDocumentation()
        {
            return "Read or write a variable, another node with the same variable name will have the same value";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(object) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(object) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Variable", "store", "read", "write" };
        }

        protected override void DoReset()
        {
            variables[varName].Reset();
        }

        protected override void OnUpdate()
        {
            if(outputs.Count != 0)
            {
                object data = variables[varName].GetData();
                if (data != null)
                {
                    outp.SetData(data);
                    ProcessAllOutputs();
                }
            }
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            variables[varName].SetData(inp.GetData());
            processAllOutputs = false;
        }

        internal override void PartialSetup()
        {

        }
    }
}

[Serializable]
public class Variable
{
    private Type type;
    [NonSerialized]
    private object data;
    List<VariableNode> subs = new List<VariableNode>();

    public void Reset()
    {
        data = null;
    }
    
    public void SetData(object data)
    {
        if(data.GetType() != type)
        {
            Debug.LogError("type missmatch in variable");
        }
        else
        {
            this.data = data;
        }
    }

    public object GetData()
    {
        return data;
    }

    public void SetDataType(Type t)
    {
        type = t;
        for(int i = 0; i < subs.Count; i++)
        {
            subs[i].OnVariableTypeChanged();
        }
    }

    public Type GetDataType()
    {
        return type;
    }

    public void Subscribe(VariableNode node)
    {
        if (!subs.Contains(node))
        {
            subs.Add(node);
        }
    }

    public void UnSubscribe(VariableNode node)
    {
        subs.Remove(node);
    }

    public Variable(Type t, object o)
    {
        type = t;
        data = o;
    }

    public Variable(Type t)
    {
        type = t;
    }

    public Variable()
    {

    }
}

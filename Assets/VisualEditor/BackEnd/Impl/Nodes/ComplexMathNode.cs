using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEditor.Visuals.Impl;
using UnityEngine;
using LoreSoft.MathExpressions;
using System.Runtime.Serialization;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    class ComplexMathNode : Node
    {
        [NonSerialized]
        MathEvaluator eval;

        string expression;
        [NonSerialized]
        public bool evaluable = true;

        public override void BeginSetup()
        {
          //  eval = new MathEvaluator();
            inputs.Add(new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) })));
            outputs.Add(new FloatOutput(this));
        }

        public void SetExpression(string expr)
        {
            if (eval == null)
                eval = new MathEvaluator();
            expression = expr;
            try
            {
                eval.Evaluate(ReplaceVariableForTest(expression));
                evaluable = true;
            }
            catch (Exception)
            {
                evaluable = false;
            }
        }

        public string GetExpression()
        {
            return expression;
        }

        private string ReplaceVariableForTest(string expr)
        {
            expr = expr.Replace("time", Time.time.ToString());
            for (int i = 0; i < inputs.Count; i++)
            {
                expr = expr.Replace("in" + (i + 1), "0");
            }
            return expr;
        }

        private string ReplaceVariables(string expr)
        {
            expr = expr.Replace("time", Time.time.ToString());
            for (int i = 0; i < inputs.Count; i++)
            {
                if(inputs[i].GetData() != null)
                {
                    expr = expr.Replace("in" + (i + 1),  "(" + inputs[i].GetData().ToString() + ")");
                }
                else
                {
                    expr = expr.Replace("in" + (i + 1), "0");
                }
            }
            return expr;
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(ComplexMathNodeVisual);
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(float) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(float) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Complex Math", "arithmetics", "add", "substract", "divide", "multiply" };
        }

        protected override void DoReset()
        {
        }

        protected override void OnUpdate()
        {
            if (evaluable)
            {
                try
                {
                    float data = (float)eval.Evaluate(ReplaceVariables(expression));
                    outputs[0].SetData(data);
                    ProcessAllOutputs();
                }
                catch (Exception)
                {
                    evaluable = false;
                }
            }
            else
            {
                SetExpression(expression);
            }
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            processAllOutputs = false;
            //this is never called here because the inputs are never 100% filled
        }

        internal override void PartialSetup()
        {
            if(inputs[inputs.Count-1].GetDataType() != null)
            {
                inputs.Add(new MultiDataInput(this, new List<Type>(new Type[] { typeof(float) })));
                SetExpression(expression);
            }
            else if (inputs.Count > 1)
            {
                Input[] copy = inputs.ToArray();
                for(int i = copy.Length - 2; i >= 0; i--)
                {
                    if(copy[i].GetDataType() == null)
                    {
                        inputs.Remove(copy[i]);
                    }
                    else
                    {
                        break;
                    }
                }
                SetExpression(expression);
            }
        }

        public override string GetDocumentation()
        {
            return "Allow you to do complex math directly in text. You can acces the data fed into the inputs by typing \"in1\", \"in2\", up to whatever inputs you put in";
        }
    }
}

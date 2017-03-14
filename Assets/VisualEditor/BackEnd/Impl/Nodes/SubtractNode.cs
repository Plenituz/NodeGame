using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class SubtractNode : AddNode
    {
        public override string GetDocumentation()
        {
            return "Substract the second input to the first";
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(SubstractNodeVisual);
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Substract", "minus", "-", "function", "math" };
        }

        protected override float FloatOperation(float f1, float f2)
        {
            return f1 - f2;
        }

        protected override Vector2 Vec2Operation(Vector2 vec1, Vector2 vec2)
        {
            return vec1 - vec2;
        }

        protected override Vector3 Vec3Operation(Vector3 vec1, Vector3 vec2)
        {
            return vec1 - vec2;
        }


    }
}

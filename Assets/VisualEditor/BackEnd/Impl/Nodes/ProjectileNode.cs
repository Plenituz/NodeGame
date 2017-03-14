using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VisualEditor.Visuals.Impl;

namespace VisualEditor.BackEnd.Impl
{
    [Serializable]
    public class ProjectileNode : Node
    {
        Vector2Input vecIn;
        FloatInput speedIn;
        GameObjectOutput objOut;
        GameObjectOutput projOut;

        [NonSerialized]
        Projectile projectile;

        public override void BeginSetup()
        {
            //inputs : direction, speed
            //output : cible touchée quand ca arrive
            vecIn = new Vector2Input(this);
            speedIn = new FloatInput(this);
            objOut = new GameObjectOutput(this);
            projOut = new GameObjectOutput(this);

            inputs.Add(vecIn);
            inputs.Add(speedIn);
            outputs.Add(projOut);
            outputs.Add(objOut);
        }

        public override Type GetAssociatedVisualClass()
        {
            return typeof(ProjectileNodeVisual);
        }

        public override string GetDocumentation()
        {
            return "Create a projectile that give you the object touched if any";
        }

        public override Type[] GetPossibleInputTypes()
        {
            return new Type[] { typeof(Vector2), typeof(float), typeof(GameObject) };
        }

        public override Type[] GetPossibleOutputTypes()
        {
            return new Type[] { typeof(GameObject) };
        }

        public override string[] GetSearchableNames()
        {
            return new string[] { "Projectile", "Raycast", "fire" };
        }

        protected override void DoReset()
        {
            if (projectile != null)
                GameObject.Destroy(projectile.gameObject);
        }

        public override void Delete()
        {
            if (projectile != null)
                GameObject.Destroy(projectile.gameObject);
        }

        protected override void OnUpdate()
        {
            if(projectile != null)
            {
                projOut.SetData(projectile.gameObject);
                ProcessSingleOutput(projOut);
            }
        }

        protected override void ProcessInputs(out bool processAllOutputs)
        {
            //create projectile, only once ?
            if(projectile == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Vector2 direct = (Vector2)vecIn.GetData();
                float speed = (float)speedIn.GetData();
                projectile = Projectile.Create(direct.normalized * speed, player.transform.position, OnHitSmtg, player);
            }

            processAllOutputs = false;
        }

        void OnHitSmtg(GameObject hit)
        {
            objOut.SetData(hit);
            ProcessSingleOutput(objOut);
        }

        internal override void PartialSetup()
        {
            
        }
    }
}

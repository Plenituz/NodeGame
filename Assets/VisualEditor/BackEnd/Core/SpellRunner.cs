using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VisualEditor.BackEnd
{
    public class SpellRunner : MonoBehaviour
    {
        public Spell spell;

        void FixedUpdate()
        {
            spell.Update();
        }
    }
}

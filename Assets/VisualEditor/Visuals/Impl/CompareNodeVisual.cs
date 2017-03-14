using UnityEngine;
using System.Collections;
using VisualEditor.Visuals;
using System.Linq;
using System;
using System.Collections.Generic;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class CompareNodeVisual : NodeVisual
    {
        static string[] convertToString = new string[] { "", ">", "<", ">=", "<=", "!=", "=", "AND", "OR", "XOR" };
        static Dictionary<string, int> convertToAction = new Dictionary<string, int> {
            {"", -1 },
            {">", 1 },
            {"<", 2 },
            {">=", 3 },
            {"<=", 4 },
            {"!=", 5 },
            {"=", 6 },
            {"AND", 7 },
            {"OR", 8 },
            {"XOR", 9 },
        };

        private int[] oldActions;
        private CompareNode mNode;
        private SplatMenu splat;

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (CompareNode)node;

            splat = SplatMenu.Create(this, new string[] {""}, OnSelect, new Vector2(100f, 100f), 150f, doStart);
            oldActions = mNode.availableActions;
        }

        string[] ConvertActionsToString()
        {
            if(mNode.availableActions == null)
            {
                return new string[] { "" };
            }
            string[] r = new string[mNode.availableActions.Length];
            // r[0] = "";
            string debug = "";
            for(int i = 0; i < r.Length; i++)
            {
                r[i] = convertToString[mNode.availableActions[i]];
                debug += r[i] + ":";
            }
            return r;
        }

        void OnSelect(string selec)
        {
            mNode.selectedAction = convertToAction[selec];
        }

        internal override void PersonnalizeSetup()
        {
            if(oldActions != null && mNode.availableActions == null)
            {
                splat.UpdateList(new string[] { "" });
                oldActions = mNode.availableActions;
                
            }
            if((oldActions == null && mNode.availableActions != null) || (oldActions != null && mNode.availableActions != null && !Enumerable.SequenceEqual(oldActions, mNode.availableActions)))
            {
                splat.UpdateList(ConvertActionsToString());
                oldActions = mNode.availableActions;
            }
        }

        static bool Match(int[] l1, int[] l2)
        {
            if (l1.Length != l2.Length)
                return false;
            for(int i = 0; i < l1.Length; i++)
            {
                if (l1[i] != l2[i])
                    return false;
            }
            return true;
        }

        internal override string GetDisplayName()
        {
            return "Compare";
        }

        internal override float GetHeight()
        {
            return 150f;
        }

        internal override float GetWidth()
        {
            return 150f;
        }

        internal override Node GetNode()
        {
            return new CompareNode();
        }

        internal override string GetInputLabel(int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    return "compare this value";
                case 1:
                    return "to this value";
            }
            return "null";
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "boolean result";
        }

        protected override void DoInitFromNode()
        {
            mNode = (CompareNode)node;
            int selectedAction = mNode.selectedAction;

            splat.UpdateList(ConvertActionsToString());

            string str = "";
            if (selectedAction != -1)
                str = convertToString[selectedAction];

            SplatItem item = splat.GetItemForText(str);
            if(item != null)
                splat.Select(item);
        }
    }
}


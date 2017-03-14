using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class GroupNodeVisual : NodeVisual
    {
        GroupNode mNode;
        string displayName;
        Vector2 oldSize;

        public Node[] initClip;
        public Vector2[] initClipPos;

        protected override void DoInitFromNode()
        {
            StartCoroutine(Preload());
        }

        IEnumerator Preload()
        {
            GameObject go = new GameObject("InputManager(group) - " + mNode.groupName);
            mNode.group = go.AddComponent<InputManager>();
            mNode.group.groupMode = true;
            mNode.group.hostIfGroup = mNode;
            mNode.group.levelUpIfGroup = host;


            yield return new WaitForEndOfFrame();
            mNode.group.Load(mNode.serializer);
            mNode.PopulateGroupInOutFromGroup();

            UpdateLabels();
            displayName = "";

            yield return new WaitForEndOfFrame();
            yield return WaitForGroupInsideToBeHiddenAndHideSelf();
        }

        IEnumerator WaitForGroupInsideToBeHiddenAndHideSelf()
        {
            List<GroupNode> groupsInside = new List<GroupNode>();
            for (int i = 0; i < mNode.group.allNodes.Count; i++)
            {
                if (mNode.group.allNodes[i].GetComponent<GroupNodeVisual>() != null)
                {
                    groupsInside.Add((GroupNode)mNode.group.allNodes[i].GetComponent<GroupNodeVisual>().node);
                }
            }
            bool allGood = false;
            while (!allGood)//wait for the groups inside me to be init
            {
                allGood = true;
                for (int i = 0; i < groupsInside.Count; i++)
                {
                    if (!groupsInside[i].group.hidden)
                    {
                        allGood = false;
                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            mNode.group.Hide();
        }

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (GroupNode)node;
            oldSize = new Vector2(GetHeight(), GetWidth());
            if(mNode.group != null)
            {
                mNode.group.levelUpIfGroup = host;
                StartCoroutine(BeginWait());
            }
            displayName = GetDisplayName();

            EventTrigger trigger = GetComponent<EventTrigger>();
            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(OnClickNode);
            trigger.triggers.Add(clickEntry);
        }

        IEnumerator BeginWait()
        {
            yield return new WaitForEndOfFrame();
           // mNode.group.Load(mNode.serializer);
            mNode.group.Paste();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            InputManager.clipboard = initClip;
            InputManager.clipboardPos = initClipPos;
            yield return WaitForGroupInsideToBeHiddenAndHideSelf();
        }

        public void OnClickNode(BaseEventData eventData)
        {
            PointerEventData ev = eventData as PointerEventData;
            if (ev == null)
                return;
            if(ev.clickCount == 2)
            {
                host.Hide();
                mNode.group.Show();
                InputManager.activeManager = mNode.group;
            }
        }

        internal override string GetDisplayName()
        {
            return mNode == null ? "Group" : mNode.groupName;
        }

        internal override float GetHeight()
        {
            return Mathf.Max(100f, Mathf.Max(node.inputs.Count, node.outputs.Count) * 25f + 10f);
        }

        internal override string GetInputLabel(int inputIndex)
        {
            try
            {
                return ((GroupInputNode)((GroupNode)node).groupIns[inputIndex].node).inName;
            }
            catch (Exception)
            {
                return "null";
            }
        }

        internal override Node GetNode()
        {
            return new GroupNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            try
            {
                return ((GroupOutputNode)((GroupNode)node).groupOuts[outputIndex].node).outName;
            }
            catch (Exception)
            {
                return "null";
            }
        }

        internal override float GetWidth()
        {
            return 100f;
        }

        internal override void PersonnalizeSetup()
        {
            if (!displayName.Equals(mNode.groupName))//Automatically update name
            {
                transform.FindChild("display text").GetComponent<Text>().text = mNode.groupName;
                displayName = mNode.groupName;
            }
            if(oldSize.y != GetHeight())
            {
                oldSize = new Vector2(GetWidth(), GetHeight());
                rectTransform.sizeDelta = oldSize;
                ((RectTransform)rectTransform.FindChild("display text")).anchoredPosition = new Vector2(0f, GetHeight() / 2 + 20f);
                PlaceInputs();
                PlaceOutputs();
                PlaceCommentText();
            }
        }
    }
}

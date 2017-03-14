using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VisualEditor.BackEnd;
using VisualEditor.BackEnd.Impl;

namespace VisualEditor.Visuals.Impl
{
    public class GroupInputNodeVisual : NodeVisual
    {
        public static Dictionary<string, Type> strToType = new Dictionary<string, Type>
        {
            { "vec2", typeof(Vector2) },
            { "vec3", typeof(Vector3) },
            { "obj", typeof(GameObject) },
            { "flt", typeof(float) },
            { "bool", typeof(bool) }
        };
        public static Dictionary<Type, string> typeToStr = new Dictionary<Type, string>
        {
            { typeof(Vector2), "vec2" },
            { typeof(Vector3), "vec3" },
            { typeof(GameObject),  "obj" },
            {  typeof(float), "flt" },
            {  typeof(bool), "bool" }
        };

        SplatMenu menu;
        InputField field;

        protected override void DoInitFromNode()
        {
            //set name and type
            GroupInputNode n = (GroupInputNode)node;
            n.SetDataType(n.dataType);
            menu.Select(menu.GetItemForText(typeToStr[n.dataType]));
            field.text = n.inName;
        }

        internal override void BeginPersonnalizeSetup()
        {
            menu = SplatMenu.Create(this, new string[] { "vec2", "vec3", "obj", "flt", "bool" }, OnSelect, new Vector2(50f, 50f), 75f, doStart);

            GameObject fieldObj = Instantiate(Resources.Load<GameObject>("InputField")) as GameObject;
            fieldObj.transform.SetParent(transform, false);
            RectTransform fieldTr = fieldObj.GetComponent<RectTransform>();
            fieldTr.anchoredPosition = new Vector2(0f, -56f);

            field = fieldObj.GetComponent<InputField>();
            ((Text)field.placeholder).text = "input name";
            field.text = "input";
            field.characterLimit = 20;
            field.textComponent.alignment = TextAnchor.MiddleCenter;
            field.onValueChanged.AddListener(OnNameChanged);
        }

        void OnNameChanged(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                ((GroupInputNode)node).inName = s;
            }
        }

        void OnSelect(string s)
        {
            ((GroupInputNode)node).SetDataType(strToType[s]);
        }

        internal override string GetDisplayName()
        {
            return "Group Input";
        }

        internal override float GetHeight()
        {
            return 75f;
        }

        internal override string GetInputLabel(int inputIndex)
        {
            return "null";
        }

        internal override Node GetNode()
        {
            return new GroupInputNode();
        }

        internal override string GetOutputLabel(int outputIndex)
        {
            return "group input data";
        }

        internal override float GetWidth()
        {
            return 75f;
        }

        internal override void PersonnalizeSetup()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VisualEditor.BackEnd;

namespace VisualEditor.Visuals
{
    public abstract class ListenerBaseNodeVisual : NodeVisual
    {
        private static Material mat;

        public ListenerBaseNode mNode;
        protected Image clickPoint;

        protected Vector2 destiWorld;
        protected Vector2 fromWorld;
        protected NodeVisual listenTo;

        private Vector2 oldPos = Vector2.zero;
        private Vector2 oldDesti;
        private bool drawLine = false;

        protected override void DoInitFromNode()
        {
            mNode.ListenTo(mNode.listenTo);

            //Update from
            Vector3 frout;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(),
                transform.position, Camera.main, out frout);
            fromWorld = frout;
            
            if (mNode.listenTo != null)
            {
                var q = from swag in FindObjectsOfType<NodeVisual>()
                        where swag.node == mNode.listenTo
                        select swag;
                listenTo = q.FirstOrDefault();//find the nodevisual
                drawLine = true;
                
                //update to
                Vector3 tout;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(),
                    listenTo.transform.position, Camera.main, out tout);
                destiWorld = tout;
            }
        }

        internal override void BeginPersonnalizeSetup()
        {
            mNode = (ListenerBaseNode)node;
            if (mat == null)
            {
                Shader shader = Shader.Find("Unlit/Color");
                mat = new Material(shader);
                mat.color = Color.black;
            }
            //faire apparaitre le point qui créé une fleche quand on clique pour pointer vers une autre node TODO
            GameObject point = new GameObject("ClickPoint");
            point.transform.SetParent(transform, false);
            clickPoint = point.AddComponent<Image>();
            clickPoint.color = Color.red;
            clickPoint.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 30f);

            EventTrigger trigger = point.AddComponent<EventTrigger>();

            EventTrigger.Entry dragEntry = new EventTrigger.Entry();
            dragEntry.eventID = EventTriggerType.Drag;
            dragEntry.callback.AddListener(OnDragPoint);
            trigger.triggers.Add(dragEntry);

            EventTrigger.Entry dragStartEntry = new EventTrigger.Entry();
            dragStartEntry.eventID = EventTriggerType.BeginDrag;
            dragStartEntry.callback.AddListener(OnBeginDragPoint);
            trigger.triggers.Add(dragStartEntry);

            EventTrigger.Entry dragEndEntry = new EventTrigger.Entry();
            dragEndEntry.eventID = EventTriggerType.EndDrag;
            dragEndEntry.callback.AddListener(OnEndDragPoint);
            trigger.triggers.Add(dragEndEntry);

            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(OnClickPoint);
            trigger.triggers.Add(clickEntry);
        }

        private void OnClickPoint(BaseEventData evData)
        {
            MouseToast.MakeToastFixed(2f, "Click and drag to the target node", UnityEngine.Input.mousePosition);
        }

        protected virtual void OnBeginDragPoint(BaseEventData evData)
        {
            drawLine = true;
        }

        protected virtual void OnDragPoint(BaseEventData evData)
        {
            Vector3 tout;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(),
                UnityEngine.Input.mousePosition, Camera.main, out tout);
            destiWorld = tout;
        }

        protected virtual void OnEndDragPoint(BaseEventData evData)
        {
            PointerEventData pData = evData as PointerEventData;
            if (pData == null)
                return;

            //if smtg below, drawline stay true, otherwise drawline = false
            if (EventSystem.current.IsPointerOverGameObject())
            {
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pData, results);
                bool hit = false;
                for(int i = 0; i < results.Count; i++)
                {
                    NodeVisual r = results[i].gameObject.GetComponent<NodeVisual>();
                    if (r != null && r != this) 
                    {
                        mNode.StopListening();
                        listenTo = r;
                        mNode.listenTo = r.node;//l'ordre est important ici
                        mNode.ListenTo(r.node);
                        hit = true;
                        break;
                    }
                }
                if (!hit)
                {
                    drawLine = false;
                    listenTo = null;
                    mNode.StopListening();//lordre est important ici
                    mNode.listenTo = null;
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if((Vector2)transform.position != oldPos)
            {
                //Update from
                oldPos = transform.position;
                Vector3 frout;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(),
                    transform.position, Camera.main, out frout);
                fromWorld = frout;
            }
            if (listenTo != null && (Vector2)listenTo.transform.position != oldDesti)
            {
                //update to
                oldDesti = listenTo.transform.position;
                Vector3 tout;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(),
                    listenTo.transform.position, Camera.main, out tout);
                destiWorld = tout;
            }
        }

        void OnRenderObject()
        {
            if (!drawLine)
                return;
            mat.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            //GL.MultMatrix(transform.worldToLocalMatrix);

            // Draw lines
            GL.Begin(GL.LINES);

            GL.Vertex(fromWorld);
            GL.Vertex(destiWorld);

            GL.End();
            GL.PopMatrix();
        }
    }
}

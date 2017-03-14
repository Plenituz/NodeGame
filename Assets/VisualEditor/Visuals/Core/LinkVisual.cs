using UnityEngine;
using System.Collections;
using VisualEditor.Visuals;

namespace VisualEditor.Visuals
{
    public class LinkVisual : MonoBehaviour
    {
        private static Material mat;

        public GameObject from;//the line will be draw from "from" to "to"
        public GameObject to;
        public OutputVisual outputThatCreatedMe;//this has to be set by the OIVisual that create this object
        public InputVisual inputDestination;

        private GameObject tmpLink;//object used to point to when the to has not been set yet 
        public bool followMouse = false;
        public bool keepAlive = false;
        private RectTransform canvasRect;
        private Vector3 oldTo;
        private Vector3 oldFrom;
        private Vector3 fromScreen;
        private Vector3 toScreen;

        void Start()
        {
            canvasRect = gameObject.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        void Update()
        {
            if (mat == null)
            {
                Shader shader = Shader.Find("Unlit/Color");
                mat = new Material(shader);
                mat.color = Color.black;
            }
            if (followMouse)
            {
                Set(from, Input.mousePosition);
            }
            else if(to == null)
            {
                Destroy(gameObject);
            }

            if(from.transform.position != oldFrom)
            {
                oldFrom = from.transform.position;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, from.transform.position, Camera.main, out fromScreen);
                fromScreen.z = -5f;
            }
            if(to.transform.position != oldTo)
            {
                oldTo = to.transform.position;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, to.transform.position, Camera.main, out toScreen);
                toScreen.z = -5f;
            }
            if(!keepAlive && inputDestination != null && !outputThatCreatedMe.outputAttachedTo.destinations.Contains(inputDestination.inputAttachedTo))
            {
                //equivalent to inputDestination.Disconnect();
                //without : outputThatCreatedMe.outputAttachedTo.DisconnectFrom(inputDestination.inputAttachedTo);

                //disconnect manually because the connection has been ripped like a big 
                //if you changed something in the Disconnect method of the output, this is likely not going to work anymore. Sorry bruh.
                inputDestination.inputAttachedTo.SetIncommingDataType(null);
                inputDestination.inputAttachedTo.host.PartialSetup();
                inputDestination.outputConnectedTo = null;
                Destroy(gameObject);
                outputThatCreatedMe.links.Remove(this);
            }
        }

        public void FollowMouse(GameObject from)
        {
            followMouse = true;
            this.from = from;
            to = null;
            inputDestination = null;
        }

        public void FinishFollowingMouse(GameObject to)
        {
            followMouse = false;
            Set(from, to);
        }

        public void Set(GameObject from, GameObject to)
        {
            if (tmpLink != null)
                Destroy(tmpLink);
            this.from = from;
            this.to = to;
            inputDestination = to.GetComponent<InputVisual>();
        }

        public void Set(GameObject from, Vector3 to)
        {
            if (tmpLink == null)
            {
                tmpLink = new GameObject("tmpLink");
                tmpLink.AddComponent<RectTransform>();
                tmpLink.transform.SetParent(transform);
            }
            this.to = tmpLink;
            this.from = from;
            tmpLink.transform.position = to;
        }

        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            if (from == null || to == null)
                return;
            DrawLine();
        }

        void DrawLine()
        {
            // Apply the line material
            mat.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            //GL.MultMatrix(transform.worldToLocalMatrix);

            // Draw lines
            GL.Begin(GL.LINES);

            GL.Vertex(fromScreen);
            GL.Vertex(toScreen);

            GL.End();
            GL.PopMatrix();
        }

        private void CetteFonctionEstCoolCarElleCalculLaPositionDuTraitAPartirDeLaPositionDeLaNodeVirguleTuDevraisLUtiliser()
        {

        }
    }
}
   

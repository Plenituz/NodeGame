using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualEditor.BackEnd
{
    [Serializable]
    public abstract class ListenerBaseNode : Node
    {
        public Node listenTo;
        public abstract void ListenTo(Node node);
        public abstract void StopListening();
    }
}

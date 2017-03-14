using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using VisualEditor.BackEnd;

namespace VisualEditor.Visuals
{
    [Serializable]
    public class InputManagerSerializer
    {
        public Position2[] positions;
        public Node[] nodes;

        [NonSerialized]
        public List<Node> tmpNodes = new List<Node>();
        [NonSerialized]
        public List<Position2> tmpPos = new List<Position2>();

        public void Add(Node n, Position2 pos)
        {
            if (tmpNodes == null)
                tmpNodes = new List<Node>();
            if (tmpPos == null)
                tmpPos = new List<Position2>();

            tmpNodes.Add(n);
            tmpPos.Add(pos);
        }        

        public void PrepareForSerialization()
        {
            nodes = tmpNodes.ToArray();
            positions = tmpPos.ToArray();
        }

        public Vector2[] PositionsToVector()
        {
            Vector2[] r = new Vector2[positions.Length];
            for(int i = 0; i < r.Length; i++)
            {
                r[i] = positions[i].ToVector();
            }
            return r;
        }

        public static InputManagerSerializer LoadFromDisk(string path)
        {
            if (File.Exists(path))
            {
                FileStream file = null;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    file = File.Open(path, FileMode.Open);
                    InputManagerSerializer r = (InputManagerSerializer)bf.Deserialize(file);
                    return r;
                }
                catch (Exception e)
                {
                    MsgBox.Make("Couldn't load file (" + path + ") from disk because:\n" + e.Message);
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
            return null;
        }
    }
}

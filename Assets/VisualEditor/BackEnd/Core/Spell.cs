using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace VisualEditor.BackEnd
{
   public class Spell
    {
        public Action onStop;
        public bool running = false;

        private Node[] list;
        private string name;
        private GameObject obj;

        public Spell(Node[] list, string name)
        {
            this.list = list;
            this.name = name;
            SetHost();
        }

        public Spell(string fromDisk)
        {
            name = fromDisk;
            LoadSpellFromDisk();
            SetHost();
        }

        void SetHost()
        {
            for (int i = 0; i < list.Length; i++)
            {
                list[i].hostSpell = this;
            }
        }

        public void Run()
        {
            if (!running)
            {
                running = true;
                obj = new GameObject("Spell :" + name);
                obj.AddComponent<SpellRunner>().spell = this;
            }
        }

        public void Stop()
        {
            if (running)
            {
                running = false;
                GameObject.Destroy(obj);
                if (onStop != null)
                    onStop();
            }
        }

        public void SaveSpellToDisk()
        {
            for(int i = 0; i < list.Length; i++)
            {
                list[i].Reset();
            }

            FileStream file = null;
            try
            {
                Directory.CreateDirectory(Application.dataPath + "/Spells");

                BinaryFormatter bf = new BinaryFormatter();
                file = File.Create(Application.dataPath + "/Spells/" + name + ".Spell");
                bf.Serialize(file, list);
            }
            catch (Exception e)
            {
                MsgBox.Make("Couldn't save spell (" + name + ") to disk because:\n" + e.Message);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        private void LoadSpellFromDisk()
        {
            if (File.Exists(Application.dataPath + "/Spells/" + name + ".Spell"))
            {
                FileStream file = null;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    file = File.Open(Application.dataPath + "/Spells/" + name + ".Spell", FileMode.Open);
                    Node[] mList = (Node[])bf.Deserialize(file);
                    list = mList;
                }
                catch (Exception e)
                {
                    MsgBox.Make("Couldn't load spell (" + name + ") from disk because:\n" + e.Message);
                }
                finally
                {
                    if(file != null)
                        file.Close();
                }
            }
            else
            {
                MsgBox.Make("this spell (" + name + ") doesn't exist bro");
                Debug.LogError("spell at Spells/" + name + ".Spell" + " doesn't exist");
                return;
            }
        }
        
        public static string[] GetList()
        {
            var q = from g in Directory.GetFiles(Application.dataPath + "/Spells")
                    where g.Contains(".SpellDic") && !g.Contains(".meta")
                    select g;
            return q.ToArray();
        }

        internal void Update()
        {
            for(int i = 0; i < list.Length; i++)
            {
                list[i].Update();
            }
        }
    }
}

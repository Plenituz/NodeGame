using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace VisualEditor.BackEnd
{
    public static class TabMenu
    {
        public static Type[] allNodes;
        private static Regex cleanNameRegex;

        public static string CleanClassName(string className)
        {
            if(cleanNameRegex == null)
            {
                cleanNameRegex = new Regex(@".*\.");
            }

            string cleanName = cleanNameRegex.Replace(className, "");
            return cleanName.Replace("Single", "Decimal").Replace("GameObject", "GameCharacter").Replace("Object", "Anything");
        }

        public static Dictionary<string, Type> GetListForInputTypesAndString(Type[] types, string search)
            //return a list of node that potentially take any of these type as an input and with a name that contains the search terme
        {
            SortedDictionary<string, Type> all = new SortedDictionary<string, Type>();

            for (int i = 0; i < allNodes.Length; i++)
            {
                var v = Activator.CreateInstance(allNodes[i]);
                Node node = (Node)v;
                string s = node.GetSearchableNames().Contains(search);
                if (s.Equals(""))
                    continue;
                Type[] ts = node.GetPossibleInputTypes();

                for (int k = 0; k < ts.Length; k++)
                {
                    if (types.Contains(ts[k]))
                    {//if the given list of types contains the current type and the no
                        if (s.Equals(node.GetSearchableNames()[0]))
                        {
                            all.Add(node.GetSearchableNames()[0] + " (in " + ts[k] + ")", allNodes[i]);
                        }
                        else
                        {
                            all.Add(node.GetSearchableNames()[0] + " (" + s + ", in " + ts[k] + ")", allNodes[i]);
                        }
                        break;
                    }
                }
            }
            return Reorder(all);
        } 
        
        public static Dictionary<string, Type> GetListForOutputTypesAndString(Type[] types, string search)
        {//return a list of node that potentially take any of these type as an ouput and with a name that contains the search terme
            SortedDictionary<string, Type> all = new SortedDictionary<string, Type>();

            for (int i = 0; i < allNodes.Length; i++)
            {
                var v = Activator.CreateInstance(allNodes[i]);
                Node node = (Node)v;
                string s = node.GetSearchableNames().Contains(search);
                if (s.Equals(""))
                    continue;
                Type[] ts = node.GetPossibleOutputTypes();

                for (int k = 0; k < ts.Length; k++)
                {
                    if (types.Contains(ts[k]))
                    {//if the given list of types contains the current type and the no
                        if (s.Equals(node.GetSearchableNames()[0]))
                        {
                            all.Add(node.GetSearchableNames()[0] + " (out " + ts[k] + ")", allNodes[i]);
                        }
                        else
                        {
                            all.Add(node.GetSearchableNames()[0] + " (" + s + ", out " + ts[k] + ")", allNodes[i]);
                        }
                        break;
                    }
                }
            }

            return Reorder(all);
        }

        private static Dictionary<string, Type> Reorder(SortedDictionary<string, Type> all)
        {
            Dictionary<string, Type> fullNames = new Dictionary<string, Type>();
            Dictionary<string, Type> parenthesis = new Dictionary<string, Type>();
            foreach (KeyValuePair<string, Type> pair in all)
            {
                if (pair.Key.Contains("("))
                {
                    parenthesis.Add(pair.Key, pair.Value);
                }
                else
                {
                    fullNames.Add(pair.Key, pair.Value);
                }
            }
            foreach (KeyValuePair<string, Type> pair in parenthesis)
            {
                fullNames.Add(pair.Key, pair.Value);
            }
            return fullNames;
        }
        
        public static Dictionary<string, Type> GetListForString(string search)//returns a list of node whose name contain search
        {//the list is a dictionnary where the keys are the displayable name and the values are the type of node to create from this name
            SortedDictionary<string, Type> all = new SortedDictionary<string, Type>();

            for (int i = 0; i < allNodes.Length; i++)
            {
                var v = Activator.CreateInstance(allNodes[i]);
                string[] names = ((Node)v).GetSearchableNames();
                string s = names.Contains(search);
                if (s.Equals(""))
                    continue;
                if (s.Equals(names[0]))
                {
                    all.Add(names[0], allNodes[i]);
                }
                else
                {
                    all.Add(names[0] + " (" + s + ")", allNodes[i]);
                }
            }

            return Reorder(all);
        }

        public static void Init()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == "VisualEditor.BackEnd.Impl"
                    select t;
            allNodes = q.ToList().ToArray();
           // allNodes.ForEach(t => Debug.Log(t.Name));
        }
        //dans la list display les noms secondaire entre parenthese si ils ont été tapé : si on tape "if" on affiche "Compare (if)" is on tape Compare on affiche "Compare"

        static string Contains(this string[] s, string word)
        {
            var q = from l in s
                    where l.ToLowerInvariant().Contains(word.ToLowerInvariant())
                    select l;
            string result = q.FirstOrDefault();
            return result == null ? "" : result;
        }
    }
}


using UnityEngine;
using System.Collections;
using VisualEditor.BackEnd;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TestCompile : MonoBehaviour {

    void Start()
    {

    }

    public void Run()
    {
        Debug.Log("run");
        Spell spell = FindObjectOfType<InputManager>().GetSpell();
        spell.Run();
        Destroy(FindObjectOfType<InputManager>().gameObject);
    }

    public void SaveToDisk()
    {
        Debug.Log("SaveToDisk");
        Spell spell = FindObjectOfType<InputManager>().GetSpell();
        spell.SaveSpellToDisk();
    }

    public void LoadFromDiskAndRun()
    {
        Debug.Log("LoadFromDiskAndRun");
        Spell spell = new Spell("s");
        spell.Run();
    }

    public void SaveDic()
    {
        FindObjectOfType<InputManager>().Save();
    }

    public void LoadDic()
    {
        FindObjectOfType<InputManager>().Load("s");
    }
}

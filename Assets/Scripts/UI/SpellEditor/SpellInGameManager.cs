using UnityEngine;
using System.Collections;

public class SpellInGameManager : MonoBehaviour {
    public VisualEditor.BackEnd.Spell[] spells;


    public static SpellInGameManager Create(VisualEditor.BackEnd.Spell[] list)
    {
        GameObject go = new GameObject("Spell Manager");
        SpellInGameManager manager = go.AddComponent<SpellInGameManager>();
        manager.spells = list;
        return manager;
    }

	void Start () {
        //create UI
        GameObject canvas = Instantiate(Resources.Load<GameObject>("SpellManager UI"));
        canvas.transform.SetParent(transform, false);

        GameObject template = canvas.transform.FindChild("Template").gameObject;
        float width = template.GetComponent<RectTransform>().sizeDelta.x;

        for(int i = 0; i < spells.Length; i++)
        {
            GameObject ch = Instantiate(template);
            ch.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * i, 0f);
            ch.transform.SetParent(canvas.transform, false);
        }
        Destroy(template);
	}
	
	void Update () {
        for(int i = (int)KeyCode.Alpha1; i < (int)KeyCode.Alpha1 + spells.Length; i++)
        {
            if(Input.GetKeyDown((KeyCode)i))
            {
                VisualEditor.BackEnd.Spell sp = spells[i - (int)KeyCode.Alpha1];
                if (sp.running)
                    sp.Stop();
                else
                    sp.Run();
            }
        }
	}
}

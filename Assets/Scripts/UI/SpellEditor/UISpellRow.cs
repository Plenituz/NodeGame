using UnityEngine;
using System.Collections;

public class UISpellRow : MonoBehaviour {
    UIListSpell daMan;
    
    public void Open()
    {
        if (daMan == null)
            daMan = GetComponentInParent<UIListSpell>();
        daMan.Open(name);
    }

    public void Delete()
    {
        if (daMan == null)
            daMan = GetComponentInParent<UIListSpell>();
        daMan.Delete(name);
    }
}

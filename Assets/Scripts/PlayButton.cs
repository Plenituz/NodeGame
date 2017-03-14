using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VisualEditor.Visuals;
using VisualEditor.BackEnd;

public class PlayButton : MonoBehaviour {
    private Text text;
    public InputManager manager;

	void Start () {
        text = GetComponent<Text>();
    }

    public void OnClick()
    {
        manager.PlayStopSpell();
        UpdateText();
    }

    void UpdateText()
    {
        text.text = manager.spell != null ? "||" : ">";
    }
}

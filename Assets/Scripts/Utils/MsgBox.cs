using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class MsgBox : MonoBehaviour {
    static GameObject canvas;
    public delegate void OnButtonClick(MsgBox msgbox, object arg);

    static void Init()
    {
        canvas = Instantiate(Resources.Load<GameObject>("ToastCanvas")) as GameObject;
        canvas.name = "MsgBox Canvas";
        Destroy(canvas.GetComponent<ToastManager>());
    }

    public static MsgBox Make(string str, string[] buttons, OnButtonClick[] onButtonClicks, object[] args)
    {
        if (canvas == null)
            Init();
        GameObject g = Instantiate(Resources.Load<GameObject>("MsgBox"));
        MsgBox msg = g.GetComponent<MsgBox>();
        msg.transform.SetParent(canvas.transform, false);
        msg.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
        msg.str = str;
        msg.buttons = buttons == null ? new string[] { } : buttons;
        msg.onButtonClicks = onButtonClicks == null ? new OnButtonClick[] { } : onButtonClicks;
        msg.args = args == null ? new object[] { } : args;
        
        return msg;
    }

    public static MsgBox Make(string str)
    {
        return Make(str, new string[] { }, new OnButtonClick[] { }, new object[] { });
    }

    public string str;
    public string[] buttons;
    public OnButtonClick[] onButtonClicks;
    public object[] args;

    void Start()
    {
        Text mainText = transform.FindChild("MainText").GetComponent<Text>();
        GameObject button = transform.FindChild("Button").gameObject;
        RectTransform exit = transform.FindChild("Exit").GetComponent<RectTransform>();
        RectTransform bg = transform.FindChild("BG").GetComponent<RectTransform>();
        List<Graphic> fades = new List<Graphic>(new Graphic[] { mainText, bg.GetComponent<Image>(), exit.GetComponent<Image>() });


        mainText.text = str;

        float width = LayoutUtility.GetPreferredWidth(mainText.GetComponent<RectTransform>());
        float height = LayoutUtility.GetPreferredHeight(mainText.GetComponent<RectTransform>());
        float buttonsSize = button.GetComponent<RectTransform>().sizeDelta.y * buttons.Length;
        Vector2 size = new Vector2(width, height + buttonsSize);

        mainText.rectTransform.pivot = new Vector2(0f, 0.5f);
        mainText.rectTransform.anchoredPosition = new Vector2(-width / 2f, 0f);

        bg.sizeDelta = size + new Vector2(20f, 20f + exit.sizeDelta.y);
        bg.anchoredPosition = new Vector2(0f, -buttonsSize/2f + exit.sizeDelta.y/2f);

        exit.anchoredPosition = bg.anchoredPosition + bg.sizeDelta / 2 - exit.sizeDelta / 2;
        exit.GetComponent<Button>().onClick.AddListener(() => Close());

        transform.position += new Vector3(0f, buttonsSize/2f);

        for(int i = 0; i < buttons.Length; i++)
        {
            GameObject g = Instantiate(button);
            if(onButtonClicks.Length > i)
            {
                if(args.Length > i)
                {
                    int tmp = i;
                    g.GetComponent<Button>().onClick.AddListener(() => {
                        onButtonClicks[tmp](this, args[tmp]);
                    });

                }
                else
                {
                    int tmp = i;
                    g.GetComponent<Button>().onClick.AddListener(() => onButtonClicks[tmp](this, null));
                }
            }
            g.transform.GetChild(0).GetComponent<Text>().text = buttons[i];
            g.transform.SetParent(transform, false);
            RectTransform rect = g.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0f, -height/2f - rect.sizeDelta.y/2f - (rect.sizeDelta.y * i));

            fades.Add(g.GetComponent<Image>());
        }
        button.SetActive(false);

        for(int i = 0; i < fades.Count; i++)
        {
            fades[i].CrossFadeAlpha(0f, 0f, true);
            fades[i].CrossFadeAlpha(1f, 0.4f, true);
        }
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}

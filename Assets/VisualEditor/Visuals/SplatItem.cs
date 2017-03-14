using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SplatItem : MonoBehaviour {
    public Vector2 dockPos;//guess what, has to be set on creating this object 
    public Vector2 restPos;//these are local positions
    public bool active = true;
    public SplatMenu host;
    public Vector2 size;
    public Color color = Color.yellow;

    private RectTransform rectTransform;
    private Text textComp;
    private Image img;
    private bool preventNextClick;
    
	void Awake () {//happens before the public values are set
        rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.localPosition = restPos;
        img = gameObject.AddComponent<Image>();
        img.color = color;

        GameObject textObj = Instantiate(Resources.Load<GameObject>("BaseText")) as GameObject;
        textObj.transform.SetParent(transform, false);
        textComp = textObj.GetComponent<Text>();
        textComp.raycastTarget = false;

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener(OnClick);
        trigger.triggers.Add(clickEntry);
    }

    void Start()
    {//happens after the public values are set
        rectTransform.sizeDelta = size;

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener(OnDrag);
        gameObject.GetComponent<EventTrigger>().triggers.Add(dragEntry);
    }

    private void OnClick(BaseEventData eventData)
    {
        if (preventNextClick)
        {
            preventNextClick = false;
            return;
        }
        PointerEventData e = eventData as PointerEventData;
        if (e == null)
            return;
        if(e.button == PointerEventData.InputButton.Left)
        {
            if (host.open)
            {
                host.Select(this);
            }
            else
            {
                host.Open();
            }
        }else if(e.button == PointerEventData.InputButton.Right)
        {
            host.host.OnClick(e);
        }
       
    }

    private void OnDrag(BaseEventData eventData)
    {
        preventNextClick = true;
        host.host.OnDrag(eventData);
    }

    public void SetText(string text)
    {
        textComp.text = text;
    }

    public string GetText()
    {
        return textComp.text;
    }

    public IEnumerator GoToDockCor()
    {
        gameObject.SetActive(true);
        yield return MoveTo(dockPos);
    }

    public IEnumerator GoToRestCor(bool hideAfter)
    {
        if (hideAfter)
        {
            img.CrossFadeAlpha(0f, 0.2f, false);
            textComp.CrossFadeAlpha(0f, 0.2f, false);
        }
        yield return MoveTo(restPos);
        if (hideAfter)
        {
            gameObject.SetActive(false);
            img.CrossFadeAlpha(1f, 0f, false);
            textComp.CrossFadeAlpha(1f, 0f, false);
        }
    }

    IEnumerator MoveTo(Vector2 pos)
    {
        while(Vector2.Distance(transform.localPosition, pos) > 1f)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, pos, 0.15f);
            yield return new WaitForEndOfFrame();
        }
    }
}

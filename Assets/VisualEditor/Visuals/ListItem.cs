using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ListItem : MonoBehaviour {
    public TabMenuList host;
    public int index = -1;

    //private bool selected = false;
    private Text textComp;
    private Image bg;
    private RectTransform rectTransform;
    private RectTransform rectChild;
    private float lastDrag;
    private bool dragging = false;

	// Use this for initialization
	void Awake () {
        rectTransform = gameObject.AddComponent<RectTransform>();
        bg = gameObject.AddComponent<Image>();
        rectTransform.sizeDelta = new Vector2(100f, TabMenuList.ITEM_HEIGHT);//size of the background

        GameObject child = new GameObject("Text");
        rectChild = child.AddComponent<RectTransform>();
        child.transform.SetParent(transform, false);
        rectChild.sizeDelta = rectTransform.sizeDelta;


        textComp = child.AddComponent<Text>();
        textComp.font = Font.CreateDynamicFontFromOSFont("Arial", 30);
        textComp.color = Color.black;
        textComp.alignByGeometry = true;
        textComp.alignment = TextAnchor.MiddleLeft;

        SetSelected(false);

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();//listen to drag event
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener(OnHover);
        trigger.triggers.Add(hoverEntry);

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();//listen to drag event
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener(OnClick);
        trigger.triggers.Add(clickEntry);

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();//listen to drag event
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener(OnDrag);
        trigger.triggers.Add(dragEntry);

        EventTrigger.Entry dragBEntry = new EventTrigger.Entry();//listen to drag event
        dragBEntry.eventID = EventTriggerType.BeginDrag;
        dragBEntry.callback.AddListener(OnDragBegin);
        trigger.triggers.Add(dragBEntry);

        EventTrigger.Entry dragEEntry = new EventTrigger.Entry();//listen to drag event
        dragEEntry.eventID = EventTriggerType.EndDrag;
        dragEEntry.callback.AddListener(OnDragEnd);
        trigger.triggers.Add(dragEEntry);

        EventTrigger.Entry scrollEntry = new EventTrigger.Entry();//listen to drag event
        scrollEntry.eventID = EventTriggerType.Scroll;
        scrollEntry.callback.AddListener(OnScroll);
        trigger.triggers.Add(scrollEntry);
    }

    private void OnScroll(BaseEventData ev)
    {
        PointerEventData evv = ev as PointerEventData;
        if (evv == null)
            return;
        host.scrollView.OnScroll(evv);
    }

    void OnDragBegin(BaseEventData ev)
    {
        PointerEventData evv = ev as PointerEventData;
        if (evv == null)
            return;
        host.scrollView.OnBeginDrag(evv);
        lastDrag = Time.time;
        dragging = true;
    }

    void OnDrag(BaseEventData ev)
    {
        PointerEventData evv = ev as PointerEventData;
        if (evv == null)
            return;
        host.scrollView.OnDrag(evv);
        lastDrag = Time.time;
    }

    void OnDragEnd(BaseEventData ev)
    {
        PointerEventData evv = ev as PointerEventData;
        if (evv == null)
            return;
        host.scrollView.OnEndDrag(evv);
        lastDrag = Time.time;
        dragging = false;
    }

    void OnClick(BaseEventData eventData)
    {
        if (Time.time - lastDrag < 0.1f || dragging)
            return;
        host.Select(this);
        host.CreateSelectedNodeAndHide();
    }

    void OnHover(BaseEventData eventData)
    {
        host.Select(this);
    }

    public void SetWidth(float w)
    {
        rectChild.sizeDelta = new Vector2(rectChild.sizeDelta.x, w);
    }

    public void SetText(string text)
    {
        textComp.text = text;
    }

    public string GetText()
    {
        return textComp.text;
    }

    public void SetSelected(bool sl)
    {
       // selected = sl;
        bg.color = sl ? Color.blue : Color.white;//Set the background color if selected
    }
}

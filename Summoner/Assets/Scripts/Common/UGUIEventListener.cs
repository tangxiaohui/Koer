using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class UGUIEventListener : MonoBehaviour ,IPointerClickHandler,IPointerDownHandler,IPointerEnterHandler,
IPointerExitHandler,IDragHandler,IPointerUpHandler,ISelectHandler,IUpdateSelectedHandler
{
	public delegate void VoidDelegate (GameObject go);
    public delegate void BoolDelegate(GameObject go, bool isClick);
	public VoidDelegate onClick;
	public VoidDelegate onDown;
	public VoidDelegate onEnter;
	public VoidDelegate onExit;
	public VoidDelegate onUp;
	public VoidDelegate onDrag;
	public VoidDelegate onSelect;
	public VoidDelegate onUpdateSelect;
    public BoolDelegate onValueChanged;

    static public UGUIEventListener Get(GameObject go)
	{
        UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
        if (listener == null)
        {
            listener = go.AddComponent<UGUIEventListener>();
        }
		return listener;
	}
	public void OnPointerClick(PointerEventData eventData)
	{
        if (onClick != null)
        {
            onClick(gameObject);
        }
	}
	public void OnPointerDown (PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject);
	}
	public void OnPointerEnter (PointerEventData eventData)
    {
		if(onEnter != null) onEnter(gameObject);
	}
	public void OnPointerExit (PointerEventData eventData)
    {
		if(onExit != null) onExit(gameObject);
	}
	public void OnPointerUp (PointerEventData eventData)
    {
		if(onUp != null) onUp(gameObject);
	}
	public void OnSelect (BaseEventData eventData)
    {
		if(onSelect != null) onSelect(gameObject);
	}
	public void OnDrag(PointerEventData eventData)
	{
		if (onDrag != null) onDrag (gameObject);
	}
	public void OnUpdateSelected (BaseEventData eventData)
    {
		if(onUpdateSelect != null) onUpdateSelect(gameObject);
	}

    public void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            if (onUp != null) onUp(gameObject);
        }
    }

    public void OnApplicationFocus(bool focus)
    {
#if UNITY_EDITOR
        if (!focus)
        {
            if (onUp != null) onUp(gameObject);
        }
#endif
    }
}

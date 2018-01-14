using UnityEngine;
using UnityEngine.EventSystems;
using System;


public class UGUIEventForList : MonoBehaviour ,IPointerClickHandler,IPointerDownHandler, IPointerUpHandler
{
    const float PRESST_THRESHOLD = 0.3f;
    const float INTERVAL = 0.2f;

    public Action<GameObject> onClick;
	public Action<GameObject> onDown;
    public Action<GameObject> onPress;
    public Action<GameObject> onUp;

    bool _beginPress = false;
    bool _truelyBegin = false;
    float _elapseTime = 0;

	public void OnPointerClick(PointerEventData eventData)
	{
        if (onClick != null) onClick(gameObject);
	}
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject);
        if (onPress != null) _beginPress = true;
    }
	public void OnPointerUp (PointerEventData eventData)
    {
		if(onUp != null) onUp(gameObject);
        if (onPress != null)
        {
            _beginPress = false;
            _truelyBegin = false;
        }
    }

    public void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            if (onUp != null) onUp(gameObject);
            if (onPress != null)
            {
                _beginPress = false;
                _truelyBegin = false;
            }
        }
    }

    private void Update()
    {
        if (_beginPress)
        {
            _elapseTime += Time.deltaTime;
            if (_truelyBegin == false && _elapseTime > PRESST_THRESHOLD)
            {
                _elapseTime = 0;
                _truelyBegin = true;
            }
            if (_truelyBegin && _elapseTime > INTERVAL)
            {
                _elapseTime -= INTERVAL;
                if(onPress != null)
                    onPress(gameObject);
            }
        }
    }
}

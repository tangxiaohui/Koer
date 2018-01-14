using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UIScrollView)), CanEditMultipleObjects]
public class UIScrollViewIspector : UnityEditor.Editor
{
    UIScrollView _target;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {

        _target = target as UIScrollView;

        RequireComponent<CanvasRenderer>(_target.gameObject);
        if(_target.GetComponent<MaskableGraphic>() == null) 
            RequireComponent<UIImage>(_target.gameObject);

        if (_target.transform.Find("Viewport") == false)
        {
            _target.vertical = false;
            GameObject go = new GameObject("Viewport");
            go.transform.SetParent(_target.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            RequireComponent<Mask>(go);
            go.GetComponent<Mask>().showMaskGraphic = false;
            RequireComponent<UIImage>(go);
            Color color = Color.white;
            color.a = 0.6f;
            _target.GetComponent<UIImage>().color = color;
            _target.viewport = go.GetComponent<RectTransform>();
            _target.viewport.SetAnchor(AnchorPresets.StretchAll);
            _target.viewport.SetPivot(PivotPresets.TopLeft);
            _target.viewport.sizeDelta = Vector2.zero;
            GameObject content = new GameObject("Content");
            content.transform.SetParent(go.transform);
            content.transform.localScale = Vector3.one;
            content.transform.localPosition = Vector3.zero;
            RequireComponent<RectTransform>(content);
            _target.content = content.GetComponent<RectTransform>();

            _target.content.SetAnchor(AnchorPresets.TopLeft);
            _target.content.SetPivot(PivotPresets.TopLeft);
            _target.content.sizeDelta = _target.viewport.rect.size;
            _target.content.localPosition = Vector3.zero;
        }

    }

    void RequireComponent<T>(GameObject go) where T : Component
    {
        if (go.GetComponent<T>() == null)
            go.AddComponent<T>();
    }


    override public void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //DrawDefaultInspector();
        //target
        //EditorGUILayout.LabelField("");
        _target.content = EditorGUILayout.ObjectField("Content",_target.content,typeof(RectTransform),true) as RectTransform;
        _target.viewport = EditorGUILayout.ObjectField("Viewport", _target.viewport, typeof(RectTransform), true) as RectTransform;


        _target.horizontal = EditorGUILayout.Toggle("Horizontal", _target.horizontal);
        _target.vertical = !_target.horizontal;
        //_target.movementType
        EditorGUILayout.BeginHorizontal();
        Enum e = EditorGUILayout.EnumPopup("Movement Type",_target.movementType);
        _target.movementType = (ScrollRect.MovementType)e;
        EditorGUILayout.EndHorizontal();

        _target.elasticity = EditorGUILayout.FloatField("Elasticity", _target.elasticity);
        _target.inertia = EditorGUILayout.Toggle("Inertia", _target.inertia);

        _target.decelerationRate = EditorGUILayout.FloatField("Deceleration Rate", _target.decelerationRate);

        _target.scrollSensitivity = EditorGUILayout.FloatField("Scroll Sensitivity", _target.scrollSensitivity);

        _target.IsAutoAligning = EditorGUILayout.Toggle("IsAutoAliging", _target.IsAutoAligning);

        EditorGUILayout.LabelField("Padding");

        _target.Left = EditorGUILayout.FloatField("Left", _target.Left);
        //_target.Right = EditorGUILayout.FloatField("Left", _target.Left);
        _target.Top = EditorGUILayout.FloatField("Top", _target.Top);

        _target.Spacing = EditorGUILayout.Vector2Field("Spacing",_target.Spacing);

        _target.Template = EditorGUILayout.ObjectField("Template", _target.Template, typeof(GameObject), true) as GameObject;

        //_target = EditorGUILayout.FloatField("Left", _target.Left);
    }

}



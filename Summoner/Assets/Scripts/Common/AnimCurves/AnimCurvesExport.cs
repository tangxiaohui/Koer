using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimCurvesExportData
{
    public string _AnimationCurveName = string.Empty;
    public float _durationTime = 0.0f;
    public float _baseValue = 1.0f;
    public AnimationCurve _AnimationCurve = new AnimationCurve();
}

public class AnimCurvesExport : MonoBehaviour
{
   public List<AnimCurvesExportData> m_AnimCurvesExportData = new List<AnimCurvesExportData>();
}

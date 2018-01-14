using UnityEngine;
using DG.Tweening;

public static class DGExtension
{   
    public static Tweener DORotate(this Transform source, Vector3 endValue, float duration, int type)
    {
        return source.DORotate(endValue, duration, (RotateMode)type);
    }

    public static Tweener DORotate(this Transform source, Vector3 endValue, float duration, int type,bool continueRotate)
    {
        return source.DORotate(endValue, duration, (RotateMode)type).SetLoops(-1).SetEase(Ease.Linear);
    }
}

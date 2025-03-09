using System;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class RotationTweenner : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float durationOfRotate = 1f;
    [SerializeField] private UltEvent OnEndOfTheRotate;



    public void LaunchTweenerInGlobalCoordinates(Vector3 finalRotation)
    {
        targetTransform.DOKill(true);
        targetTransform.DORotate(finalRotation, durationOfRotate)
            .SetEase(curve)
            .OnComplete(CompleteTheTweener);
    }
    
    public void LaunchTweenerInLocalCoordinates(Vector3 finalRotation)
    {
        targetTransform.DOKill(true);
        targetTransform.DOLocalRotate(finalRotation, durationOfRotate)
            .SetEase(curve)
            .OnComplete(CompleteTheTweener);
    }

    public void CompleteTheTweener()
    {
        OnEndOfTheRotate.Invoke();
    }
}

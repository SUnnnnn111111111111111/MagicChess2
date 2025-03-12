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

    public void LaunchTweenerInLocalRandomRotation(Vector3 minRotation, Vector3 maxRotation)
    {
        Vector3 randomRotation = new Vector3(
            UnityEngine.Random.Range(minRotation.x, maxRotation.x),
            UnityEngine.Random.Range(minRotation.y, maxRotation.y),
            UnityEngine.Random.Range(minRotation.z, maxRotation.z)
        );
        
        targetTransform.DOLocalRotate(randomRotation, durationOfRotate)
            .SetEase(curve)
            .OnComplete(CompleteTheTweener);
    }
    
    public void LaunchTweenerInGlobalRandomRotation(Vector3 minRotation, Vector3 maxRotation)
    {
        Vector3 randomRotation = new Vector3(
            UnityEngine.Random.Range(minRotation.x, maxRotation.x),
            UnityEngine.Random.Range(minRotation.y, maxRotation.y),
            UnityEngine.Random.Range(minRotation.z, maxRotation.z)
        );
        
        targetTransform.DORotate(randomRotation, durationOfRotate)
            .SetEase(curve)
            .OnComplete(CompleteTheTweener);
    }

    private void CompleteTheTweener()
    {
        OnEndOfTheRotate.Invoke();
    }
}
using System;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class MoveTweenner : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float durationOfMove = 1f;
    [SerializeField] private UltEvent OnEndOfTheMove;

    public void LaunchTweenerInGlobalCoordinates(Vector3 finalPosition)
    {
        targetTransform.DOKill(true);
        targetTransform.DOLocalMove(finalPosition, durationOfMove)
            .SetEase(curve)
            .OnComplete(CompleteTheTweener); 
    }

    public void LaunchTweenerInLocalCoordinates(Vector3 localOffset)
    {
        Vector3 newLocalPosition = targetTransform.localPosition + localOffset;
        targetTransform.DOLocalMove(newLocalPosition, durationOfMove);
    }


    private void CompleteTheTweener()
    {
        OnEndOfTheMove.Invoke();
    }
}
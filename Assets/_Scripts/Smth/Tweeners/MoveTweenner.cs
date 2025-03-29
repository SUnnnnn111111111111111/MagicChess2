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

    public void LaunchTweenerInLocalRandomCoordinates(Vector3 minOffset, Vector3 maxOffset)
    {
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(minOffset.x, maxOffset.x),
            UnityEngine.Random.Range(minOffset.y, maxOffset.y),
            UnityEngine.Random.Range(minOffset.z, maxOffset.z)
        );
        
        Vector3 newLocalPosition = targetTransform.localPosition + randomOffset;
        
        targetTransform.DOLocalMove(newLocalPosition, durationOfMove)
            .SetEase(curve)
            .OnComplete(CompleteTheTweener);
    }

    private void CompleteTheTweener()
    {
        OnEndOfTheMove.Invoke();
    }
}
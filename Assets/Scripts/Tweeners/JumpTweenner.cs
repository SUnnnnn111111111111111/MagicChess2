using System;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class JumpTweenner : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Ease curve;
    [SerializeField] private float durationOfJump = 1f;
    [SerializeField] private float forceOfJump = 1f;
    [SerializeField] private int countOfJump = 2;
    [SerializeField] private UltEvent OnEndOfTheJump;



    public void LaunchTheTweener(Vector3 finalPosition)
    {
        targetTransform.DOKill(true);
        targetTransform.DOJump(finalPosition, forceOfJump, countOfJump, durationOfJump)
            .SetEase(curve)
            .OnComplete(CompleteTheTweener);
    }

    public void CompleteTheTweener()
    {
        OnEndOfTheJump.Invoke();
    }
}

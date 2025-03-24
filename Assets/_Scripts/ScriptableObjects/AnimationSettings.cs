using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "AnimationSettings", menuName = "Chess/AnimationSettings")]
public class AnimationSettings : ScriptableObject
{
    public float moveDuration = 0.5f;
    public float rotateDuration = 0.2f;
    public Ease moveEase = Ease.Linear;
    public float jumpPower = 0f;
    public int jumpCount = 1;
}
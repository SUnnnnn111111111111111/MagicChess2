using System;
using UnityEngine;


[CreateAssetMenu(fileName = "TimeScalerSetter", menuName = "Settings/TimeScalerSetter")]
public class TimeScalerSetter : ScriptableObject
{
    [SerializeField] [Range(0, 100)] float timeScale = 1f;

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}

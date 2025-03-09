using UnityEngine;
using UltEvents;

public class AwakeHandler : MonoBehaviour
{
    [SerializeField] private UltEvent  OnAwake;

    void Awake()
    {
        OnAwake.Invoke();
    }
}
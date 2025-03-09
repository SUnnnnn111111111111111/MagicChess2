using UnityEngine;
using UltEvents;

public class DestroyHandler : MonoBehaviour
{
    [SerializeField] private UltEvent  _OnDestroy;

    void OnDestroy()
    {
        _OnDestroy.Invoke();
    }
}
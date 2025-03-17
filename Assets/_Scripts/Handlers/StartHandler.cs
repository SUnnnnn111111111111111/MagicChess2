using UnityEngine;
using UltEvents;

public class StartHandler : MonoBehaviour
{
    [SerializeField] private UltEvent  OnStart;

    void Start()
    {
        OnStart.Invoke();
    }
}

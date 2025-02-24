using UnityEngine;
using UltEvents;

public class EventLauncher : MonoBehaviour
{
    [SerializeField] private UltEvent Event;

    void LaunchEvent()
    {
        Event.Invoke();
    }
}

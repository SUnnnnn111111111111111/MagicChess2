using UltEvents;
using UnityEngine;

public class HandlerOnEnableOnDisable : MonoBehaviour
{
    [SerializeField] private UltEvent WhenTurnedOn;
    [SerializeField] private UltEvent WhenTurnedOff;
    
    void OnEnable()
    {
        WhenTurnedOn.Invoke();
    }

    void OnDisable()
    {
        WhenTurnedOff.Invoke();
    }
}

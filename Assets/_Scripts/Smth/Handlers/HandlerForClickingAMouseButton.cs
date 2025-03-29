using UnityEngine;
using UltEvents;

public class HandlerForClickingAMouseButton : MonoBehaviour
{
    public bool isActive;
    [SerializeField] private UltEvent  WhenClickOnAnObject;


    void OnMouseDown()
    {
        if(isActive)
            WhenClickOnAnObject.Invoke();
    }
}

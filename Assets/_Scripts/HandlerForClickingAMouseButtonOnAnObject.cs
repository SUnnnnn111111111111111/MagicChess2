using UnityEngine;
using UltEvents;

public class HandlerForClickingAMouseButtonOnAnObject : MonoBehaviour
{
    public bool isActive;
    [SerializeField] private UltEvent  WhenClickOnAnObject;


    void OnMouseDown()
    {
        if(isActive)
        WhenClickOnAnObject.Invoke();
    }
}

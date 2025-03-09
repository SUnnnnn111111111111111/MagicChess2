using UnityEngine;
using UltEvents;

public class CollisionHandler : MonoBehaviour
{
    public bool isActive = true;
    [SerializeField] private UltEvent<GameObject> OnEnter;

    public void OnCollisionEnter(Collision other)
    {
        if(isActive)
            OnEnter.Invoke(other.gameObject);
    }
}
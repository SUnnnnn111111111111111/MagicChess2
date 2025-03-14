using UnityEngine;

public class ForceApplier : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Vector3 forceDirection = Vector3.up;
    [SerializeField] private float forceMagnitude = 10f;
    
    public void AddForce()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object is not assigned!");
            return;
        }

        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Target Object does not have a Rigidbody component!");
            return;
        }
        
        Vector3 force = forceDirection.normalized * forceMagnitude;
        rb.AddForce(force, ForceMode.Force);
        Debug.Log($"Force applied to {targetObject.name}: {force}");
    }
    
    public void AddForce(Vector3 customDirection, float customMagnitude)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object is not assigned!");
            return;
        }

        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Target Object does not have a Rigidbody component!");
            return;
        }
        
        Vector3 force = customDirection.normalized * customMagnitude;
        rb.AddForce(force, ForceMode.Force);
        Debug.Log($"Custom force applied to {targetObject.name}: {force}");
    }
    
    public void AddForceToRandomDirectionForce(Vector3 minDirectionRange, Vector3 maxDirectionRange, float minForceMagnitude, float maxForceMagnitude)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object is not assigned!");
            return;
        }

        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Target Object does not have a Rigidbody component!");
            return;
        }
        
        float randomX = Random.Range(minDirectionRange.x, maxDirectionRange.x);
        float randomY = Random.Range(minDirectionRange.y, maxDirectionRange.y);
        float randomZ = Random.Range(minDirectionRange.z, maxDirectionRange.z);
        Vector3 randomDirection = new Vector3(randomX, randomY, randomZ);
        
        float randomForceMagnitude = Random.Range(minForceMagnitude, maxForceMagnitude);
        
        Vector3 force = randomDirection.normalized * randomForceMagnitude;
        rb.AddForce(force, ForceMode.Force);
        Debug.Log($"Random force applied to {targetObject.name}: {force}");
    }
}
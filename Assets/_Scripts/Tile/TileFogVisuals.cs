using UnityEngine;

public class TileFogVisuals : MonoBehaviour
{
    [Header("Fog of War Effect")]
    [SerializeField] private GameObject fogOfWarEffect;

    public void SetFog(bool isActive)
    {
        if (fogOfWarEffect != null)
            fogOfWarEffect.SetActive(isActive);
    }
}
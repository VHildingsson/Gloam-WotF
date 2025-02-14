using UnityEngine;

public class SilhouetteEffect : MonoBehaviour
{
    public Material blackMaterial; // Assign your black material in the Inspector

    void Start()
    {
        ApplyBlackMaterial();
    }

    void ApplyBlackMaterial()
    {
        // Get all SpriteRenderer components in the children of this GameObject
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.material = blackMaterial;
        }
    }
}


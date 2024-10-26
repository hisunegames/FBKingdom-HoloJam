using UnityEngine;

public class CorruptionMaskApplier : MonoBehaviour
{
    public Camera secondaryCamera; // Assign the second camera in the Inspector
    public Material tilemapMaterial; // Assign the tilemap's material in the Inspector

    void Start()
    {
        if (secondaryCamera.targetTexture != null && tilemapMaterial != null)
        {
            tilemapMaterial.SetTexture("_CorruptionMask", secondaryCamera.targetTexture);
        }
    }
}

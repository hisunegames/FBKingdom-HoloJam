using UnityEngine;

public class CameraCorruptionEffectController : MonoBehaviour
{
    public Material corruptionMaterial; // Assign your corruption material here

    void Update()
    {
        if (corruptionMaterial != null)
        {
            // Update the Camera Position in the shader with the camera's position
            corruptionMaterial.SetVector("_CameraPosition", Camera.main.transform.position);
        }
    }
}

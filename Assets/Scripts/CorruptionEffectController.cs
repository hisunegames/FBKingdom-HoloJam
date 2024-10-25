using UnityEngine;

[ExecuteAlways]
public class CorruptionController : MonoBehaviour
{
    public Material corruptionMaterial; // Assign the shader material in the inspector

    public GameObject point1Object; // Corruption start point object
    public GameObject point2Object; // Corruption mid point object
    public GameObject point3Object; // Corruption end point object

    public Color corruptionColor = Color.red; // Color to apply from the midpoint to the endpoint
    [Range(0, 1)]
    public float corruptionStrength = 0.5f; // Control the corruption strength
    [Range(2, 256)]
    public int posterizeLevels = 8; // Control the posterization levels

    void Update()
    {
        if (corruptionMaterial != null && point1Object != null && point2Object != null && point3Object != null)
        {
            // Set positions of the three points
            corruptionMaterial.SetVector("_Point1", point1Object.transform.position);
            corruptionMaterial.SetVector("_Point2", point2Object.transform.position);
            corruptionMaterial.SetVector("_Point3", point3Object.transform.position);

            // Set the corruption color
            corruptionMaterial.SetColor("_CorruptionColor", corruptionColor);

            // Set the corruption strength
            corruptionMaterial.SetFloat("_CorruptionStrength", corruptionStrength);

            // Set the posterization levels
            corruptionMaterial.SetInt("_PosterizeLevels", posterizeLevels);
        }
    }
}

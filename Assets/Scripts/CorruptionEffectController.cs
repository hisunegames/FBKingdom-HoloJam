using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CorruptionEffectController : MonoBehaviour
{
    public Material corruptionMaterial;
    public Color corruptionColor = Color.red;
    [Range(0, 1)]
    public float corruptionStrength = 0.5f;
    [Range(2, 256)]
    public int posterizeLevels = 8;

    [System.Serializable]
    public class CorruptionPoint
    {
        public GameObject pointObject;
        public float radius;
    }

    public List<CorruptionPoint> corruptionPoints = new List<CorruptionPoint>();
    private ComputeBuffer corruptionBuffer;

    void Update()
    {
        if (corruptionMaterial != null && corruptionPoints.Count > 0)
        {
            corruptionMaterial.SetColor("_CorruptionColor", corruptionColor);
            corruptionMaterial.SetFloat("_CorruptionStrength", corruptionStrength);
            corruptionMaterial.SetInt("_PosterizeLevels", posterizeLevels);

            // Prepare corruption points data
            var pointsData = new Vector4[corruptionPoints.Count];
            for (int i = 0; i < corruptionPoints.Count; i++)
            {
                if (corruptionPoints[i].pointObject != null)
                {
                    Vector3 pos = corruptionPoints[i].pointObject.transform.position;
                    pointsData[i] = new Vector4(pos.x, pos.y, pos.z, corruptionPoints[i].radius);
                }
            }

            // Create and update the ComputeBuffer
            if (corruptionBuffer != null) corruptionBuffer.Release();
            corruptionBuffer = new ComputeBuffer(pointsData.Length, sizeof(float) * 4);
            corruptionBuffer.SetData(pointsData);

            // Bind the buffer to the shader
            corruptionMaterial.SetBuffer("_CorruptionPoints", corruptionBuffer);
        }
    }

    void OnDisable()
    {
        // Release the ComputeBuffer when not needed
        if (corruptionBuffer != null)
        {
            corruptionBuffer.Release();
        }
    }
}

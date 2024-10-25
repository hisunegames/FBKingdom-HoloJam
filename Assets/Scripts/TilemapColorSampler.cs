using System.Collections;
using UnityEngine;

public class TilemapColorSampler : MonoBehaviour
{
    public RenderTexture colorTexture; // Set this to the RenderTexture where your tilemap shader outputs colors
    public Camera colorCamera;         // Camera rendering the tilemap to the RenderTexture
    public ParticleSystem p_System;

    private Vector3 lastPosition;
    public float checkInterval = 0.2f;

    void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(UpdateColorAtInterval());
        if (!colorTexture || !colorCamera)
        {
            Debug.LogError("ColorTexture and ColorCamera need to be set up in the Inspector.");
            return;
        }
        colorCamera.targetTexture = colorTexture;
    }

    void Update()
    {
        // Sample the color beneath the player's position
        Color sampledColor = SampleColorAtPosition(transform.position);

        // Apply the color to the particle system
        ApplyColorToParticleSystem(sampledColor);
    }

    Color SampleColorAtPosition(Vector3 worldPosition)
    {
        // Convert world position to screen position
        Vector3 screenPos = colorCamera.WorldToScreenPoint(worldPosition);

        // Check if the screen position is within the bounds of the RenderTexture
        if (screenPos.x < 0 || screenPos.x >= colorTexture.width || screenPos.y < 0 || screenPos.y >= colorTexture.height)
        {
            // Debug.LogWarning("Screen position is outside the bounds of the RenderTexture.");
            return Color.clear; // Return a default color if out of bounds
        }

        // Read color at this position from the RenderTexture
        RenderTexture.active = colorTexture;
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(screenPos.x, screenPos.y, 1, 1), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        Color sampledColor = texture.GetPixel(0, 0);
        Destroy(texture);  // Clean up the temporary texture
        return sampledColor;
    }

    void ApplyColorToParticleSystem(Color color)
    {
        var main = p_System.main;
        main.startColor = color;  // Apply sampled color to particles

        // If you want particles to gradually change color based on the new sampled color
        // you could use a ColorOverLifetime module with Gradient or animate the change over time
    }

    IEnumerator UpdateColorAtInterval()
    {
        while (true)
        {
            // Only update color if the player has moved to a new tile
            if (Vector3.Distance(lastPosition, transform.position) > 0.1f)
            {
                Color sampledColor = SampleColorAtPosition(transform.position);
                ApplyColorToParticleSystem(sampledColor);
                lastPosition = transform.position;
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }
}

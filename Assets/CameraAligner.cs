using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraAligner : MonoBehaviour
{
    public Camera mainCamera;      // Reference to the main camera
    public Tilemap tilemap;        // Reference to the tilemap
    public RenderTexture renderTexture; // Reference to the render texture for corruption

    private void Start()
    {
        AlignCameraToTilemap();
    }

    private void AlignCameraToTilemap()
    {
        if (tilemap == null || mainCamera == null || renderTexture == null)
        {
            Debug.LogError("Missing references in CameraAligner.");
            return;
        }

        // Get the bounds of the tilemap
        Bounds tilemapBounds = tilemap.localBounds;

        // Calculate the center position of the tilemap
        Vector3 center = tilemapBounds.center;
        center.z = mainCamera.transform.position.z; // Keep the camera's z position

        // Set the camera position to the center of the tilemap
        mainCamera.transform.position = center;

        // Adjust the camera size based on the tilemap bounds
        float height = tilemapBounds.size.y;
        float width = tilemapBounds.size.x;

        // Set orthographic size for the camera
        if (mainCamera.orthographic)
        {
            // Set orthographic size to half the height of the tilemap
            mainCamera.orthographicSize = height / 2.0f;
        }

        // Update the Render Texture size to match the tilemap size
        renderTexture.width = Mathf.RoundToInt(width);
        renderTexture.height = Mathf.RoundToInt(height);
        renderTexture.Create();

        // Set the camera's target texture
        mainCamera.targetTexture = renderTexture;
    }
}

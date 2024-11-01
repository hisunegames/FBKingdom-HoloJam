using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WallCloseTrigger : MonoBehaviour
{
    public GameObject wall;
    public CinemachineCamera bossCamera;
    public PixelPerfectCamera pixelPerfectCamera;
    public int resolutionMultiplier;
    public EnemyAI boss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if (!other.CompareTag("Player")) return;

        wall.SetActive(true);
        CameraManager.instance.SetActiveCamera(bossCamera);

        // Change the resolution of the Pixel Perfect Camera
        pixelPerfectCamera.refResolutionX *= resolutionMultiplier;
        pixelPerfectCamera.refResolutionY *= resolutionMultiplier;

        boss.canBoss = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        this.gameObject.SetActive(false);
    }

}

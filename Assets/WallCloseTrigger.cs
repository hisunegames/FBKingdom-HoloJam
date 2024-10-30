using Unity.Cinemachine;
using UnityEngine;

public class WallCloseTrigger : MonoBehaviour
{
    public GameObject wall;
    public CinemachineCamera bossCamera;
    private void OnTriggerEnter2D(Collider2D other)
    {
        wall.SetActive(true);
        CameraManager.instance.SetActiveCamera(bossCamera);
    }
}

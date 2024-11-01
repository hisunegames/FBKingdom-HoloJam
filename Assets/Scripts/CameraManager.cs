using UnityEngine;
using System;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineCamera[] cameras;
    public CinemachineCamera defaultCamera;
    public CinemachineCamera fubuzillaCamera;
    public CinemachineCamera currentCamera;


    public void SetActiveCamera(CinemachineCamera currentCamera)
    {
        foreach (CinemachineCamera camera in cameras)
        {
            camera.Priority = camera == currentCamera ? 10 : 0;
        }
    }

    public void DisableCameraFollow()
    {
        var cinemachineComponent = currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
        if (cinemachineComponent != null)
        {
            cinemachineComponent.enabled = false;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        cameras = GetComponentsInChildren<CinemachineCamera>();
        currentCamera = defaultCamera;
        SetActiveCamera(currentCamera);
    }



    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     if (currentCamera == defaultCamera)
        //     {
        //         currentCamera = fubuzillaCamera;
        //     }
        //     else
        //     {
        //         currentCamera = defaultCamera;
        //     }
        //     SetActiveCamera(currentCamera);
        // }
    }

}

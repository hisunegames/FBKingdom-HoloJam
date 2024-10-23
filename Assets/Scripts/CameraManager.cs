using UnityEngine;
using Unity.Cinemachine;
using System;

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

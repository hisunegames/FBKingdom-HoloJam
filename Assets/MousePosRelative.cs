using System;
using UnityEngine;

public class MousePosRelative : MonoBehaviour
{
    [SerializeField]
    private GameObject parallaxCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0; // Set z to 0 if working in 2D

        // Normalize mouse position x to -1 to 1
        float normalizedMouseX = (mousePosition.x / Screen.width) * 100;

        // Add normalized x to the camera position
        parallaxCamera.transform.position = new Vector3(normalizedMouseX, transform.position.y, transform.position.z);
    }
}

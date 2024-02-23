using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("----- Camera Settings -----")]
    [SerializeField] float cameraSensitivity;
    [SerializeField] float cameraVerticalRotationLimit = 80f;
    [SerializeField] bool cameraInvertY = false;

    // Horizontal Camera Rotation
    private float rotationX = 0f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        if (cameraInvertY)
            rotationX += mouseY;
        else
            rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -cameraVerticalRotationLimit, cameraVerticalRotationLimit);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}

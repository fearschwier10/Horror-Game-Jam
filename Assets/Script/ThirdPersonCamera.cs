using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform playerTransform;  // The player's transform
    public Vector3 cameraOffset;       // Offset of the camera from the player
    public float cameraRotationSpeed = 5f;  // Speed of camera rotation

    private float currentZoom = 10f;
    public float cameraZoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    void Update()
    {
        // Zoom in/out with mouse scroll wheel
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void LateUpdate()
    {
        // Rotate the camera around the player with the right mouse button
        if (Input.GetMouseButton(1))
        {
            float horizontal = Input.GetAxis("Mouse X") * cameraRotationSpeed;
            playerTransform.Rotate(Vector3.up * horizontal);
        }

        // Calculate the desired camera position
        Vector3 desiredPosition = playerTransform.position - cameraOffset * currentZoom;
        transform.position = desiredPosition;

        // Look at the player
        transform.LookAt(playerTransform.position + Vector3.up * 1.5f);  // Adjust the height of the look-at point as needed
    }
}

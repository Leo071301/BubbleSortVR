using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Read in mouse middle zoom button using new Input System
public class CameraZoom : MonoBehaviour
{

    private CinemachineCamera Camera;
    private CinemachineThirdPersonFollow CinemachineThirdPersonFollow;
    public  InputActionReference zoomInput;



    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float smoothSpeed = 10f;

    private float targetCameraDistance; // cache

    private void Awake()
    {
        Camera = GetComponent<CinemachineCamera>();

        // Cache the ThirdPersonFollow component from the Cinemachine Camera
        CinemachineThirdPersonFollow = GetComponent<CinemachineThirdPersonFollow>();
    }

    private void Start()
    {
        targetCameraDistance = CinemachineThirdPersonFollow.CameraDistance;
        
    }

    private void Update()
    {
        if (!Camera.IsLive) return;

        // Read scroll input (Y component of mouse scroll)
        float scrollValue = zoomInput.action.ReadValue<Vector2>().y;

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            // Normalize scroll input across different devices/platforms and adjust target distance
            targetCameraDistance -= Mathf.Sign(scrollValue) * zoomSpeed * Time.unscaledDeltaTime;
            targetCameraDistance = Mathf.Clamp(targetCameraDistance, minDistance, maxDistance);
        }

        // Smoothly interpolate (Lerp) to the target distance
        CinemachineThirdPersonFollow.CameraDistance = Mathf.Lerp(
            CinemachineThirdPersonFollow.CameraDistance,
            targetCameraDistance,
            smoothSpeed * Time.deltaTime
        );
    }
}
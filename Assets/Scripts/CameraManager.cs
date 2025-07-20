using Unity.Mathematics;
using Unity.Properties;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform playerTransform;  
    public Transform cameraPivot;      // The object the camera uses to pivot (look up and down)
    public Transform cameraTransform;  // Transform of the actual camera object in the scene

    public LayerMask collisionLayers;  // Layers the camera will collide with

    private float defaultPosition;
    public float cameraCollisionOffSet = 0.2f; // How much the camera will jump off of objects its colliding with
    public float cameraCollisionRadius = 0.2f;
    public float minimumCollisionOffSet = 0.2f;

    private Vector3 cameraVectorPosition;

    public float smoothingTime = 0.2f;
    public float cameraLookSpeed = 20;
    public float cameraPivotSpeed = 20;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    [SerializeField] float minPivotAngle;
    [SerializeField] float maxPivotAngle;

    public float lookAngle; // Rotating up and down
    public float pivotAngle; // Rotating left and right 

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowPlayer();
        RotateCamera();
        HandleCameraCollisions();
    }
    private void FollowPlayer()
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, playerTransform.position, ref cameraFollowVelocity, smoothingTime);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle; // rotation around y axis
        targetRotation = Quaternion.Euler(rotation); // Quaternion.Euler(0,lookAngle,0)
        transform.rotation = targetRotation; // transform.rotation = Quaternion.Euler(0,lookAngle,0)

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation); // Quaternion.Euler(pivotAngle, 0, 0)
        cameraPivot.localRotation = targetRotation;  // cameraPivot.localRotation = Quaternion.Euler(pivotAngle, 0, 0)  
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast
            (cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

}


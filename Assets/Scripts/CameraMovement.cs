using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Transform cameraPivot;
    public Transform cameraTransform;
    public LayerMask collisionLayers;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;
    public float cameraCollisionRadius = 0.2f;
    public float cameraCollisionOffset = 0.1f;
    public float minCollisionOffset = 0.1f;

    public float lookAngle;
    public float pivotAngle;
    public float minPivotAngle = -35;
    public float maxPivotAngle = 35;

    Vector3 cameraFollowVelocity = Vector3.zero;
    Vector3 cameraVectorPosition;
    PlayerControls playerControls;
    Vector2 input;
    public float defaultPosition;

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Camera.performed += i => input = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    private void Awake()
    {
        defaultPosition = cameraTransform.localPosition.z;
    }

    private void LateUpdate()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    public void FollowTarget()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref cameraFollowVelocity, cameraFollowSpeed);
    }

    public void RotateCamera()
    {
        lookAngle += input.x * cameraLookSpeed;

        pivotAngle -=  input.y * cameraPivotSpeed;
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        transform.rotation = Quaternion.Euler(new Vector3(0, lookAngle, 0));
        cameraPivot.localRotation = Quaternion.Euler(new Vector3(pivotAngle, 0, 0));
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;

        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition -= minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}

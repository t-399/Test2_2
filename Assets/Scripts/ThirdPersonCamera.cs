using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Distance")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float zoomSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Smoothing")]
    [SerializeField] private float rotationSmoothTime = 0.05f;
    [SerializeField] private float positionSmoothTime = 0.05f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float collisionRadius = 0.2f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursor = true;

    private float yaw;
    private float pitch;
    private float currentDistance;
    private Vector3 positionVelocity;
    private float yawVelocity;
    private float pitchVelocity;
    private float smoothYaw;
    private float smoothPitch;

    private void Awake()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
        currentDistance = distance;

        SetCursorLocked(lockCursor);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            lockCursor = !lockCursor;
            SetCursorLocked(lockCursor);
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        ReadInput();

        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawVelocity, rotationSmoothTime);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchVelocity, rotationSmoothTime);

        Quaternion rotation = Quaternion.Euler(smoothPitch, smoothYaw, 0f);
        Vector3 pivot = target.position + targetOffset;
        Vector3 desiredPosition = pivot - rotation * Vector3.forward * currentDistance;

        desiredPosition = HandleCollision(pivot, desiredPosition);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, positionSmoothTime);
        transform.rotation = rotation;
    }

    private void ReadInput()
    {
        if (Mouse.current != null && (!lockCursor || Application.isFocused))
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw += mouseDelta.x * mouseSensitivity;
            pitch -= mouseDelta.y * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
                distance = Mathf.Clamp(distance - scroll * zoomSpeed * 0.001f, minDistance, maxDistance);
        }

        currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * 10f);
    }

    private Vector3 HandleCollision(Vector3 pivot, Vector3 desiredPosition)
    {
        Vector3 direction = desiredPosition - pivot;
        float length = direction.magnitude;
        if (length < 0.0001f)
            return desiredPosition;

        direction /= length;

        if (Physics.SphereCast(pivot, collisionRadius, direction, out RaycastHit hit, length, collisionMask, QueryTriggerInteraction.Ignore))
            return pivot + direction * Mathf.Max(hit.distance - collisionRadius, minDistance * 0.1f);

        return desiredPosition;
    }

    private void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private float verticalVelocity;
    private static readonly int SpeedHash = Animator.StringToHash("speed");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Vector2 input = ReadMoveInput();
        bool isRunning = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

        Vector3 moveDirection = Vector3.zero;
        float currentSpeed = 0f;

        if (input.sqrMagnitude > 0.0001f)
        {
            Vector3 forward = cameraTransform != null ? cameraTransform.forward : transform.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            moveDirection = (forward * input.y + right * input.x).normalized;
            currentSpeed = isRunning ? runSpeed : walkSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        ApplyGravity();

        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
            animator.SetFloat(SpeedHash, currentSpeed);
    }

    private Vector2 ReadMoveInput()
    {
        if (Keyboard.current == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;

        return Vector2.ClampMagnitude(input, 1f);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;
    }
}

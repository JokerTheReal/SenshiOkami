    using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float sprintingSpeed = 7;
    public float runningSpeed = 5;
    public float rotationSpeed = 15;

    public Transform movementTarget;

    Vector2 input;
    Vector3 moveDirection;
    Rigidbody playerRigidbody;
    PlayerControls playerControls;

    Animator animator;
    float moveAmount;

    public bool isSprinting;
    public bool isSprintButtonTrigger;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);   
        animator.CrossFade(targetAnimation, 0.2f);
    }

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += OnMovementPerformed;
            playerControls.PlayerActions.Sprint.performed += i => isSprintButtonTrigger = true;
            playerControls.PlayerActions.Sprint.canceled += i => isSprintButtonTrigger = false;
        }

        playerControls.Enable();
    }

    void OnMovementPerformed(CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        moveAmount = Mathf.Clamp01(Mathf.Abs(input.x) + Mathf.Abs(input.y));
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    public void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimations();
    }

    public void HandleMovement()
    {
        moveDirection = movementTarget.forward * input.y;
        moveDirection += movementTarget.right * input.x;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = 1f;

        isSprinting = isSprintButtonTrigger && moveAmount > 0;

        if (isSprinting)
        {
            speed = sprintingSpeed;
        } 
        else
        {
            if (moveAmount >= 0.5f)
            {
                speed = runningSpeed;
            }
            else
            {
                speed = walkingSpeed;
            }
        }

        moveDirection *= speed;

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    public void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = movementTarget.forward * input.y;
        targetDirection += movementTarget.right * input.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    public void UpdateAnimations()
    {
        animator.SetFloat("Vertical", isSprinting ? 2 : Snap(moveAmount), 0.1f, Time.deltaTime);
    }

    private static float Snap(float movement)
    {
        float snappedValue = 0;

        if (movement > 0 && movement < 0.55f)
        {
            snappedValue = 0.5f;
        }
        else if (movement > 0.55f)
        {
            snappedValue = 1;
        }
        else if (movement < 0 && movement > -0.55f)
        {
            snappedValue = -0.5f;
        }
        else if (movement < -0.55f)
        {
            snappedValue = -1;
        }

        return snappedValue;
    }
}

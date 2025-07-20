using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Vector3 movementDirection;
    Transform cameraObject;
    Rigidbody playerRigidBody;
    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;

    [Header("Falling")]
    public float inAirTimer;
    public float fallingSpeed;
    public float leapingVelocity;
    public float rayCastHeightOffSet = 0.2f;
    public LayerMask groundLayer;

    public bool isSprinting;
    public bool isGrounded;

    [Header("Movement and rotation speeds")]
    public float moveSpeed;
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float playerRotationSpeed = 30;

    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
    }


    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
            return;

        HandleRotation();
        HandleMovement();
    }
    private void HandleMovement()
    {
        movementDirection = (cameraObject.forward * inputManager.verticalInput) + (cameraObject.right * inputManager.horizontalInput);
        movementDirection.Normalize();
        movementDirection.y = 0;
        movementDirection = movementDirection * moveSpeed;

        Vector3 movementVelocity = movementDirection;
        playerRigidBody.linearVelocity = movementVelocity;
    }

    private void HandleRotation()
    {

        Vector3 targetDirection = Vector3.zero;
        targetDirection = (cameraObject.forward * inputManager.verticalInput) + (cameraObject.right * inputManager.horizontalInput);
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, playerRotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;


    }

    public void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffSet;
        Debug.DrawRay(rayCastOrigin, -Vector3.up, Color.blue);

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, 0.2f, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayerTargetAnimation("LandingAnimation", true);
            }

            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (!isGrounded)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayerTargetAnimation("FallingAnimation", true);
            }

            inAirTimer += Time.deltaTime;

            playerRigidBody.AddForce(transform.forward * leapingVelocity);
            playerRigidBody.AddForce(-Vector3.up * fallingSpeed * Time.deltaTime);
        }
        
        }
    }


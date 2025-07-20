using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    PlayerLocomotion playerLocomotion;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float verticalInput;
    public float horizontalInput;

    public float cameraInputY;
    public float cameraInputX;

    public float moveAmount;

    public bool sprint_Input;

    private void Awake()
    {
        animatorManager = GetComponentInChildren<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += ctx => sprint_Input = true;
            playerControls.PlayerActions.Sprint.canceled += ctx => sprint_Input = false;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.HandleAnimatorValues(0, moveAmount);
        // moveAmount is the absolute sum of horizontal and vertical input
        // so even if you only press horizontal keys, animation will play.
        // first parameter is 0 because in blendtree, we only care about vertical
        // for moving forward
        // but if we want strafing, we can't have it as 
    }

    private void HandleSprintingInput()
    {
        if (sprint_Input && moveAmount >= 0.55)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }
}

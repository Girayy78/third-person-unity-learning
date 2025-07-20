using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;

    public bool isInteracting;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        inputManager = FindFirstObjectByType<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindFirstObjectByType<CameraManager>();
    }
    private void Update()
    {
        inputManager.HandleAllInputs();
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Log(transform.position.y);
    }

    // When moving a RigidBody, do it in FixedUpdate
    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();

        isInteracting = animator.GetBool("isInteracting");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}

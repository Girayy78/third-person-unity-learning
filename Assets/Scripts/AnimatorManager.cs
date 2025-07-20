using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    Animator animator;
    PlayerLocomotion playerLocomotion;

    int horizontal;
    int vertical;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();

        // Converts the parameter name "Horizontal" into a unique integer hash.
        // Using the hash instead of the string improves performance when accessing Animator parameters,
        // especially when setting parameters frequently in Update or similar methods.
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }
    
    public void PlayerTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    public void HandleAnimatorValues(float horizontalMovement, float verticalMovement)
    {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if(horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if(horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.55f;
        }
        else if(horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
            playerLocomotion.moveSpeed = playerLocomotion.walkingSpeed;
        }
        else if (verticalMovement > 0.55f && playerLocomotion.isSprinting)
        {
            snappedVertical = 1;
            playerLocomotion.moveSpeed = playerLocomotion.sprintingSpeed;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
            playerLocomotion.moveSpeed = playerLocomotion.runningSpeed;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
            playerLocomotion.moveSpeed = playerLocomotion.walkingSpeed;
        }
        else if (verticalMovement < -0.55f && playerLocomotion.isSprinting)
        {
            snappedVertical = -1;
            playerLocomotion.moveSpeed = playerLocomotion.sprintingSpeed;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
            playerLocomotion.moveSpeed = playerLocomotion.runningSpeed;
        }
        else
        {
            snappedVertical = 0;
            playerLocomotion.moveSpeed = 0;
        }
        #endregion

        if (playerLocomotion.isSprinting)
        {
            snappedVertical = 2;
        }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
}

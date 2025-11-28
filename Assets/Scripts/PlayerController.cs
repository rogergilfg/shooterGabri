using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const string MOVEMENT_ACTION_NAME = "Move";
    private const string ANIMATOR_HORIZONTAL = "Horizontal";
    private const string ANIMATOR_VERTICAL = "Vertical";
    private const string ANIMATOR_SHOOTING = "Shooting";

    private Animator animator;
    private PlayerInput playerInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector2 leftStickInput = playerInput.actions[MOVEMENT_ACTION_NAME].ReadValue<Vector2>();
        animator.SetFloat(ANIMATOR_HORIZONTAL, leftStickInput.x);
        animator.SetFloat(ANIMATOR_VERTICAL, leftStickInput.y);
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            animator.SetBool(ANIMATOR_SHOOTING, true);
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            animator.SetBool(ANIMATOR_SHOOTING, false);
        }
    }
}
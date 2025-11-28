using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    private const string MOVEMENT_ACTION_NAME = "Move";
    private const string ANIMATOR_HORIZONTAL = "Horizontal";
    private const string ANIMATOR_VERTICAL = "Vertical";

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

    public void Shoot()
    {
        
    }
}
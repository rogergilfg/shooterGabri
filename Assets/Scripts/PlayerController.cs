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
    private Rigidbody rb;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float sensibility;
    [SerializeField]
    private Transform followTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector2 leftStickInput = playerInput.actions[MOVEMENT_ACTION_NAME].ReadValue<Vector2>();
        animator.SetFloat(ANIMATOR_HORIZONTAL, leftStickInput.x);
        animator.SetFloat(ANIMATOR_VERTICAL, leftStickInput.y);
        Vector3 movement = ((transform.forward * leftStickInput.y) + (transform.right * leftStickInput.x)) * speed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }

    private void LateUpdate()
    {
        Vector2 lookInput = playerInput.actions["Look"].ReadValue<Vector2>();
        followTarget.localEulerAngles += new Vector3(lookInput.y*sensibility*Time.deltaTime, 0, 0);
        transform.eulerAngles += new Vector3(0, lookInput.x * sensibility * Time.deltaTime, 0);
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
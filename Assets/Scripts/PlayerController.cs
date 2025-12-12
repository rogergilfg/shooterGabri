using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const string MOVEMENT_ACTION_NAME = "Move";
    private const string ANIMATOR_HORIZONTAL = "Horizontal";
    private const string ANIMATOR_VERTICAL = "Vertical";
    private const string ANIMATOR_SHOOTING = "Shooting";
    private const string ANIMATOR_RELOADING = "Reload";

    private Animator animator;
    private PlayerInput playerInput;
    private Rigidbody rb;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float sensibility;
    [SerializeField]
    private Transform followTarget;
    private int weaponIndex;
    private LevelManager lm;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
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
            GameManager.instance.GetGameData.Weapons[GameManager.instance.GetGameData.WeaponIndex].Triggered();
            lm.UpdateBullets();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            animator.SetBool(ANIMATOR_SHOOTING, false);
            GameManager.instance.GetGameData.Weapons[GameManager.instance.GetGameData.WeaponIndex].TriggerReleased();
        }
    }

    public void Reload(InputAction.CallbackContext callBackContext)
    {
        if (callBackContext.phase == InputActionPhase.Performed)
        {
            animator.SetTrigger(ANIMATOR_RELOADING);
            GameManager.instance.GetGameData.Weapons[GameManager.instance.GetGameData.WeaponIndex].Reload();
            lm.UpdateBullets();
            playerInput.actions["Shoot"].Disable();
        }
    }

    public void CanShoot()
    {
        playerInput.actions["Shoot"].Enable();

    }
}
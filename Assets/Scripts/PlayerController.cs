using System;
using System.Collections;
using System.Net.NetworkInformation;
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
    [SerializeField]
    private float healthSpeed;
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform grenadeSpawnPoint;
    [SerializeField]
    private float throwForce;
    [SerializeField] private GameObject grenadePrefab;

    [SerializeField]
    private Transform leftHand, rightHand;

    private int weaponIndex;
    private LevelManager lm;
    [SerializeField]
    private float timeToStartHealth;
    private IEnumerator corrutinaCurar;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        lineRenderer = grenadeSpawnPoint.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Vector2 leftStickInput = playerInput.actions[MOVEMENT_ACTION_NAME].ReadValue<Vector2>();
        animator.SetFloat(ANIMATOR_HORIZONTAL, leftStickInput.x);
        animator.SetFloat(ANIMATOR_VERTICAL, leftStickInput.y);
        Vector3 movement = ((transform.forward * leftStickInput.y) + (transform.right * leftStickInput.x)) * speed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        //Line renderer grenade
        if (lineRenderer.enabled == true)
        {
            Vector3 speed = (Camera.main.transform.forward + Vector3.up) * throwForce;
            lineRenderer.positionCount = 100;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float t /*t de tiempo*/ = i * 0.1f;
                Vector3 position = grenadeSpawnPoint.position + speed * t + 0.5f * Physics.gravity * t * t;
                lineRenderer.SetPosition(i, position);
            }
        }
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
            playerInput.actions["ThrowGrenade"].Disable();
        }
    }

    public void CanShoot()
    {
        playerInput.actions["Shoot"].Enable();

    }

    public void TakeDamage(float damage)
    {
        if(corrutinaCurar != null)
        {
            StopCoroutine(corrutinaCurar);
        }


        GameManager.instance.GetGameData.CurrentLife -= damage;
        if(GameManager.instance.GetGameData.CurrentLife < 0)
        {
            //Muerte
            GameObject ragdollPrefab = Resources.Load<GameObject>("SwatRagdoll");
            Instantiate(ragdollPrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
        else
        {
            corrutinaCurar = Health();
            StartCoroutine(corrutinaCurar);
        }
        lm.UpdateLife();
    }

    IEnumerator Health()
    {
        yield return new WaitForSeconds(timeToStartHealth);
        while (GameManager.instance.GetGameData.CurrentLife < GameManager.instance.GetGameData.MaxLife) 
        {
            GameManager.instance.GetGameData.CurrentLife = Mathf.Clamp(GameManager.instance.GetGameData.CurrentLife + (healthSpeed * Time.deltaTime), 0, GameManager.instance.GetGameData.MaxLife);
            lm.UpdateLife();
            yield return null;
        }
    }

    public void ThrowGrenade(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetBool("Granade", true);
            lineRenderer.enabled = true;
            GameManager.instance.GetGameData.Weapons[GameManager.instance.GetGameData.WeaponIndex].transform.parent = leftHand;
            Instantiate(grenadePrefab, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation, grenadeSpawnPoint);
            playerInput.actions["Shoot"].Disable();
        }

        if(context.canceled)
        {
            animator.SetBool("Granade", false);
        }
    }

    public void SoltarGrenade()
    {
        lineRenderer.enabled = false;
        var grenade = grenadeSpawnPoint.GetChild(0).transform;
        grenade.parent = null;

        var grenadeRigidbody = grenade.GetComponent<Rigidbody>();
        var grenadeCollider = grenade.GetComponent<Collider>();

        grenadeCollider.enabled = true;
        grenadeRigidbody.isKinematic = false;
        grenadeRigidbody.linearVelocity = (Camera.main.transform.forward + Vector3.up) * throwForce;
        grenade.GetComponent<Grenade>().countDownActive = true;
    }

    public void FinishGrenade()
    {
        CanShoot();
        GameManager.instance.GetGameData.Weapons[GameManager.instance.GetGameData.WeaponIndex].transform.parent = rightHand;
    }
}
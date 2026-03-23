using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MultiplayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private const string MOVEMENT_ACTION_NAME = "Move";

    [SerializeField] private float speed;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Balas")]
    [SerializeField] private int maxMagazine = 30;
    [SerializeField] private int totalBullets = 90;
    [SerializeField] private float reloadTime = 2f;

    [Header("Reposición de Balas")]
    [SerializeField] private int balasReposicion = 30;
    [SerializeField] private float tiempoReposicion = 10f;
    private float reposicionTimer = 0f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoBalas;
    [SerializeField] private TextMeshProUGUI textoEstado;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private Animator animator;
    private float life;
    private bool ejemplo;

    private int currentBullets;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting == true)
        {
            stream.SendNext(ejemplo);
            stream.SendNext(life);
        }
        else
        {
            ejemplo = (bool)stream.ReceiveNext();
            life = (float)stream.ReceiveNext();
        }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentBullets = maxMagazine;
        reposicionTimer = tiempoReposicion;

        if (photonView.IsMine == true)
        {
            Camera.main.GetComponent<CameraMultiplayerController>().SetPlayer(transform);
            ActualizarUI();
        }
    }

    void Update()
    {
        if (photonView.IsMine == false) return;

        // Movimiento
        Vector2 leftStickInput = playerInput.actions[MOVEMENT_ACTION_NAME].ReadValue<Vector2>();
        Vector3 arriba = Vector3.forward + Vector3.left;
        Vector3 derecha = Vector3.forward + Vector3.right;
        Vector3 movement = ((arriba * leftStickInput.y) + (derecha * leftStickInput.x)) * speed;

        animator.SetBool("Run", movement != Vector3.zero);
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        // Mirar
        float y = Camera.main.GetComponent<CameraMultiplayerController>().camOffset.y;
        Vector2 mousePos = playerInput.actions["LookCenital"].ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, y));
        Vector3 playerRot = transform.eulerAngles;
        transform.LookAt(worldPos);
        transform.eulerAngles = new Vector3(playerRot.x, transform.eulerAngles.y, playerRot.z);

        // Disparo
        if (playerInput.actions["Shoot"].WasPressedThisFrame() && !isReloading)
            TryShoot();

        // Recarga
        if (playerInput.actions["Reload"].WasPressedThisFrame())
            TryReload();

        // Timer de recarga
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (textoEstado != null)
                textoEstado.text = $"Recargando... {reloadTimer:F1}s";

            if (reloadTimer <= 0f)
                FinishReload();
        }

        // Reposición automática de balas cada X segundos
        reposicionTimer -= Time.deltaTime;
        if (reposicionTimer <= 0f)
        {
            totalBullets += balasReposicion;
            reposicionTimer = tiempoReposicion;
            ActualizarUI();
            Debug.Log($"[Ammo] +{balasReposicion} balas. Total: {totalBullets}");
        }
    }

    // ── DISPARO ──────────────────────────────────────────────

    private void TryShoot()
    {
        if (currentBullets <= 0)
        {
            if (textoEstado != null) textoEstado.text = "¡Sin balas! [R] para recargar";
            return;
        }

        currentBullets--;
        ActualizarUI();

        GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bulletClone.GetComponent<Rigidbody>().linearVelocity = bulletClone.transform.forward * 40f;

        MultiBullet bullet = bulletClone.GetComponent<MultiBullet>();
        if (bullet != null)
        {
            bullet.owner = photonView.Owner;
            bullet.damage = 10f;
        }

        photonView.RPC("CopyShoot", RpcTarget.Others);

        if (currentBullets <= 0 && totalBullets > 0)
            StartReload();
    }

    [PunRPC]
    void CopyShoot()
    {
        GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bulletClone.GetComponent<Rigidbody>().linearVelocity = bulletClone.transform.forward * 40f;
    }

    // ── RECARGA ──────────────────────────────────────────────

    private void TryReload()
    {
        if (isReloading) return;
        if (currentBullets == maxMagazine) return;
        if (totalBullets <= 0)
        {
            if (textoEstado != null) textoEstado.text = "¡No tienes balas de reserva!";
            return;
        }

        StartReload();
    }

    private void StartReload()
    {
        isReloading = true;
        reloadTimer = reloadTime;
        animator.SetTrigger("Reload");
    }

    private void FinishReload()
    {
        isReloading = false;

        int bulletsNeeded = maxMagazine - currentBullets;
        if (bulletsNeeded <= totalBullets)
        {
            currentBullets = maxMagazine;
            totalBullets -= bulletsNeeded;
        }
        else
        {
            currentBullets += totalBullets;
            totalBullets = 0;
        }

        ActualizarUI();
    }

    // ── UI ───────────────────────────────────────────────────

    private void ActualizarUI()
    {
        if (textoBalas != null)
            textoBalas.text = $"{currentBullets} / {totalBullets}";

        if (textoEstado != null)
            textoEstado.text = "";
    }

    // ── DAÑO ─────────────────────────────────────────────────

    public void TakeDamage(float damage, Player player)
    {
        life -= damage;
        if (life <= 0)
        {
            if (player.CustomProperties.ContainsKey("Muertes") == true)
            {
                player.CustomProperties.TryGetValue("Muertes", out object muertes);
                int deaths = (int)muertes + 1;
                Hashtable muerdeaths = new Hashtable { { "Muertes", deaths } };
                player.SetCustomProperties(muerdeaths);
            }
        }
    }

    void VerMuertes()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            PhotonNetwork.CurrentRoom.Players[i].CustomProperties.TryGetValue("Muertes", out var nombreVariable);
        }
    }
}
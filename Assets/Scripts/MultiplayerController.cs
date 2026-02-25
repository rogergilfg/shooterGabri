using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private const string MOVEMENT_ACTION_NAME = "Move";
    private const string ANIMATOR_HORIZONTAL = "Horizontal";
    private const string ANIMATOR_VERTICAL = "Vertical";

    [SerializeField] private float speed;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private Animator animator;
    private float life;
    bool ejemplo;


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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if(photonView.IsMine == true)
        {
            Camera.main.GetComponent<CameraMultiplayerController>().SetPlayer(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(photonView.IsMine == true)
        {
            //Movimiento
            Vector2 leftStickInput = playerInput.actions[MOVEMENT_ACTION_NAME].ReadValue<Vector2>();
            Vector3 arriba = Vector3.forward + Vector3.left;
            Vector3 derecha = Vector3.forward + Vector3.right;
            Vector3 movement = ((arriba * leftStickInput.y) + (derecha * leftStickInput.x)) * speed;

            animator.SetBool("Run", movement != Vector3.zero);


                rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

            //Mirar
            float y = Camera.main.GetComponent<CameraMultiplayerController>().camOffset.y;
            Vector2 mousePos = playerInput.actions["LookCenital"].ReadValue<Vector2>();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, y));
            Vector3 plauerRot = transform.eulerAngles;
            transform.LookAt(worldPos);
            transform.eulerAngles = new Vector3(plauerRot.x, transform.eulerAngles.y, plauerRot.z);
        }   
    }

    /// <summary>
    /// Opcion 1 de disparo online
    /// </summary>
    /// <param name="context"></param>
    public void Shoot(InputAction.CallbackContext context)
    {
        if(photonView.IsMine == true)
        {
            if (context.performed == true)
            {
                GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                bulletClone.GetComponent<Rigidbody>().linearVelocity = bulletClone.transform.forward * 20;

                photonView.RPC("CopyShoot", RpcTarget.Others);
            }
        }
    }

    [PunRPC]

    void CopyShoot()
    {
        GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bulletClone.GetComponent<Rigidbody>().linearVelocity = bulletClone.transform.forward * 20;
    }

    /// <summary>
    /// Opcion 2 de disparo online
    /// Que se sincronice la bala en todos lados
    /// </summary>
    public void Shoot2(InputAction.CallbackContext context)
    {
        if (photonView.IsMine == true)
        {
            if(context.performed == true)
            {
                GameObject bulletClone = PhotonNetwork.Instantiate("MultiplayerBullet", bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                bulletClone.GetComponent<Rigidbody>().linearVelocity = bulletClone.transform.forward * 20;
            }
        }
    }

    //EN LA BALA/////////////////////////////////////////////////////////

    private void OnCollisionEnter(Collision other)
    {
        if(photonView.IsMine == true)
        {
            if(other.gameObject.tag == "Enemy")
            {
                //other.gameObject.GetComponent<EnemyController>().TakeDamage(10, photonView.Owner);
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////
    //EN EL SCRIPT DEL ENEMIGO

    void TakeDamage(float damage, Player player)
    {
        life -= damage;
        if(life <= 0)
        {
            int deaths;
            //Muerte
            if (player.CustomProperties.ContainsKey("Muertes") == true)
            {
                object muertes;
                player.CustomProperties.TryGetValue("Muertes", out muertes);
                deaths = (int)muertes;
                deaths += 1;

                Hashtable muerdeaths = new Hashtable { { "Muertes", deaths } };
                player.SetCustomProperties(muerdeaths);
            }
            else
            {
                deaths = 1;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    ///
    void VerMuertes()
    {
        for(int i = 0; i<PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            PhotonNetwork.CurrentRoom.Players[i].CustomProperties.TryGetValue("Muertes", out var nombrevariable);
        }
    }

}

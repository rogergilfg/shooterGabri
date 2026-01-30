using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private const string MOVEMENT_ACTION_NAME = "Move";
    private const string ANIMATOR_HORIZONTAL = "Horizontal";
    private const string ANIMATOR_VERTICAL = "Vertical";

    [SerializeField] private float speed;

    private Rigidbody rb;
    private PlayerInput playerInput;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
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
            Vector2 leftStickInput = playerInput.actions[MOVEMENT_ACTION_NAME].ReadValue<Vector2>();
            Vector3 movement = ((transform.forward * leftStickInput.y) + (transform.right * leftStickInput.x)) * speed;
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }   
    }
}

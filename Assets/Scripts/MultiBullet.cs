using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

public class MultiBullet : MonoBehaviourPunCallbacks, IPunObservable
{

    public float damage;
    public Player owner;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Enemy");
            if (photonView.IsMine == true)
            {
                if (other.gameObject.tag == "Enemy")
                {
                    other.gameObject.GetComponent<EnemyMultiplayerController>().TakeDamage(10, photonView.Owner);
                }
            }
        }
    }
}

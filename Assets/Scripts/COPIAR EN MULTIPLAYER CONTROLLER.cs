using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class COPIARENMULTIPLAYERCONTROLLER : MonoBehaviourPunCallbacks, IPunObservable
{
    private float life;
    bool ejemplo;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting == true)
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
}

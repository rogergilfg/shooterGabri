using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiBullet : MonoBehaviourPun
{
    public float damage = 10f;
    public Player owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Si");
            other.gameObject.GetComponent<EnemyMultiplayerController>()
                ?.TakeDamage(damage, owner);

            Destroy(gameObject);
        }
    }
}

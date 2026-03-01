using UnityEngine;
using UnityEngine.Video;

public class Bullet : MonoBehaviour
{
    public float damage;
    [SerializeField] private GameObject bulletHolePrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Auch");
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
            //Instanciariamos sangre
        }

        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Auch puto niño");
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            //Instanciariamos sangre
        }
        else
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.back, collision.GetContact(0).normal);
            GameObject bulletHoleClone = Instantiate(bulletHolePrefab, collision.GetContact(0).point, rotation, collision.transform);
            bulletHoleClone.transform.localPosition += new Vector3(0, 0, 0.8f);
            Destroy(bulletHoleClone, 5f);
        }

            Destroy(gameObject);
    }

    //ESTO ES PARA EL VIDEO

    VideoPlayer videoPlayer;

    private void OnTriggerEnter(Collider other)
    {
        videoPlayer.Play();
        videoPlayer.Stop();
        videoPlayer.Pause();
    }
}

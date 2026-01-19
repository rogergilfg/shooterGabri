using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Auch");
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Auch puto niño");
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}

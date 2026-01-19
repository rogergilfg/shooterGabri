using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float explosionRadius, damage, timeToExplode, explosionTimer, knockBackForce;
    [SerializeField] private GameObject explosionPrefab;
    public bool countDownActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(countDownActive == true)
        {
            explosionTimer += Time.deltaTime;

            if (explosionTimer > timeToExplode)
            {
                KABOOM();
            }
        }
    }

    void KABOOM()
    {
        //SFX
        GameObject vfx = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(knockBackForce, transform.position, explosionRadius);
                if (colliders[i].gameObject.tag == "Enemy")
                {
                    colliders[i].GetComponent<EnemyController>().TakeDamage(damage);
                }
                else if (colliders[i].gameObject.tag=="Player")
                {
                    colliders[i].GetComponent<PlayerController>().TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }
}
